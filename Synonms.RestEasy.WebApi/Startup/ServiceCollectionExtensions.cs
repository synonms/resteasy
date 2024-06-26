using System.Reflection;
using Synonms.RestEasy.Core.Domain.Events;
using Synonms.RestEasy.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using Synonms.RestEasy.Core.Application;
using Synonms.RestEasy.Core.Attributes;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Environment;
using Synonms.RestEasy.Core.Persistence;
using Synonms.RestEasy.Core.Schema.Errors;
using Synonms.RestEasy.Core.Schema.Forms;
using Synonms.RestEasy.WebApi.Application;
using Synonms.RestEasy.WebApi.Auth;
using Synonms.RestEasy.WebApi.Controllers.Auth;
using Synonms.RestEasy.WebApi.Correlation;
using Synonms.RestEasy.WebApi.Domain.Events;
using Synonms.RestEasy.WebApi.Http;
using Synonms.RestEasy.WebApi.Mediation.Commands;
using Synonms.RestEasy.WebApi.Mediation.Queries;
using Synonms.RestEasy.WebApi.Pipeline.Products;
using Synonms.RestEasy.WebApi.Pipeline.Products.Context;
using Synonms.RestEasy.WebApi.Pipeline.Products.Resolution;
using Synonms.RestEasy.WebApi.Pipeline.Tenants;
using Synonms.RestEasy.WebApi.Pipeline.Tenants.Context;
using Synonms.RestEasy.WebApi.Pipeline.Tenants.Resolution;
using Synonms.RestEasy.WebApi.Pipeline.Users;
using Synonms.RestEasy.WebApi.Pipeline.Users.Context;
using Synonms.RestEasy.WebApi.Pipeline.Users.Resolution;
using Synonms.RestEasy.WebApi.Routing;
using Synonms.RestEasy.WebApi.Swashbuckle;

namespace Synonms.RestEasy.WebApi.Startup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRestEasyFramework<TUser, TProduct, TTenant>(this IServiceCollection serviceCollection, RestEasyOptions options)
        where TUser : RestEasyUser
        where TProduct : RestEasyProduct
        where TTenant : RestEasyTenant
    {
        serviceCollection.AddHttpContextAccessor();

        IResourceDirectory resourceDirectory = new ResourceDirectory(options.Assemblies);
        serviceCollection.AddSingleton(resourceDirectory);

        IRouteNameProvider routeNameProvider = new RouteNameProvider();
        serviceCollection.AddSingleton(routeNameProvider);
        
        serviceCollection.AddSingleton<IErrorCollectionDocumentFactory, ErrorCollectionDocumentFactory>();
        serviceCollection.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        
        serviceCollection.AddScoped<OptionsMiddleware>();
        serviceCollection.AddScoped<CorrelationMiddleware>();
        serviceCollection.AddScoped<RestEasyBearerTokenRelayHandler>();
        serviceCollection.AddScoped<RestEasyCorrelationRelayHandler>();

        serviceCollection.AddScoped<IRouteGenerator, HttpRouteGenerator>();
        serviceCollection.AddScoped(typeof(ICreateFormDocumentFactory<,>), typeof(CreateFormDocumentFactory<,>));
        serviceCollection.AddScoped(typeof(IEditFormDocumentFactory<,>), typeof(EditFormDocumentFactory<,>));
        
        foreach ((string _, IResourceDirectory.AggregateRootLayout aggregateRootLayout) in resourceDirectory.GetAllRoots())
        { 
            serviceCollection
                .RegisterRequestHandlers(aggregateRootLayout)
                .RegisterResourceMappers(aggregateRootLayout);
        }

        foreach (IResourceDirectory.AggregateMemberLayout aggregateMemberLayout in resourceDirectory.GetAllMembers())
        { 
            serviceCollection
                .RegisterChildResourceMappers(aggregateMemberLayout);
        }
        
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(IMediator), typeof(Mediator), ServiceLifetime.Transient));
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(ISender), sp => sp.GetRequiredService<IMediator>(), ServiceLifetime.Transient));
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(IPublisher), sp => sp.GetRequiredService<IMediator>(), ServiceLifetime.Transient));

        // Replace default mappers where an explicit one is provided
        serviceCollection.RegisterAllImplementationsOf(typeof(IResourceMapper<,>), serviceCollection.AddSingleton, options.Assemblies);

        serviceCollection.AddScoped<IChildResourceMapperFactory, ChildResourceMapperFactory>();
        serviceCollection.AddScoped<IResourceMapperFactory, ResourceMapperFactory>();

        serviceCollection.RegisterApplicationDependenciesFrom(options.Assemblies);
        serviceCollection.RegisterDomainDependenciesFrom(options.Assemblies);

        if (options.CorsConfiguration is not null)
        {
            serviceCollection.WithCorsPolicy(options.CorsConfiguration);
        }

        serviceCollection.AddSingleton<IDateProvider, UtcDateProvider>();

        serviceCollection.WithUsers<TUser>();
        serviceCollection.WithTenants<TUser, TTenant>();
        serviceCollection.WithProducts<TUser, TProduct>();
        serviceCollection.AddScoped<PermissionsMiddleware<TUser, TProduct, TTenant>>();
        
        serviceCollection.WithOpenApi(options.SwaggerGenConfigurationAction);
        
        AuthenticationBuilder authenticationBuilder = serviceCollection.AddAuthentication(options.DefaultAuthenticationScheme);
        options.AuthenticationConfigurationAction?.Invoke(authenticationBuilder);
        
        serviceCollection.AddAuthorization(authorizationOptions =>
        {
            IEnumerable<Assembly> assemblies = options.Assemblies.Concat(new[] { typeof(MigrationsPolicyRegistrar).Assembly });
            
            authorizationOptions.AddRestEasyAuthorisationPolicies(assemblies);
            options.AuthorizationConfiguration?.Invoke(authorizationOptions);
        });

        serviceCollection.WithControllers(options, routeNameProvider, resourceDirectory);
        
        return serviceCollection;
    }
    
    public static IServiceCollection RegisterAllImplementationsOf(this IServiceCollection serviceCollection, Type interfaceType, Func<Type, Type, IServiceCollection> registrationFunc, params Assembly[] assemblies)
    {
        if (interfaceType.IsGenericType)
        {
            assemblies
                .SelectMany(assembly => assembly.GetImplementationsOfGenericInterface(interfaceType))
                .ToList()
                .ForEach(implementationType =>
                {
                    List<Type> implementedInterfaces = implementationType.GetInterfaces().ToList();
                    implementedInterfaces.ForEach(implementedInterface =>
                    {
                        registrationFunc(implementedInterface, implementationType);
                    });
                });
        }
        else
        {
            assemblies
                .SelectMany(assembly => assembly.GetImplementationsOfNonGenericInterface(interfaceType))
                .ToList()
                .ForEach(implementationType =>
                {
                    List<Type> implementedInterfaces = implementationType.GetInterfaces().ToList();
                    implementedInterfaces.ForEach(implementedInterface =>
                    {
                        registrationFunc(implementedInterface, implementationType);
                    });
                });
        }

        return serviceCollection;
    }
    
    private static IServiceCollection RegisterRequestHandlers(this IServiceCollection serviceCollection, IResourceDirectory.AggregateRootLayout aggregateRootLayout)
    {
        RestEasyResourceAttribute? resourceAttribute = aggregateRootLayout.AggregateRootType.GetCustomAttribute<RestEasyResourceAttribute>();

        if (resourceAttribute is null)
        {
            return serviceCollection;
        }
        
        Type findResourceRequestType = typeof(FindResourceRequest<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
        Type findResourceResponseType = typeof(FindResourceResponse<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
        Type findResourceRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(findResourceRequestType, findResourceResponseType);
        Type findResourceRequestHandlerImplementationType = typeof(FindResourceRequestProcessor<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);

        serviceCollection.AddTransient(findResourceRequestHandlerInterfaceType, findResourceRequestHandlerImplementationType);
        
        Type readResourceCollectionRequestType = typeof(ReadResourceCollectionRequest<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
        Type readResourceCollectionResponseType = typeof(ReadResourceCollectionResponse<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
        Type readResourceCollectionRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(readResourceCollectionRequestType, readResourceCollectionResponseType);
        Type readResourceCollectionRequestHandlerImplementationType = typeof(ReadResourceCollectionRequestProcessor<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);

        serviceCollection.AddTransient(readResourceCollectionRequestHandlerInterfaceType, readResourceCollectionRequestHandlerImplementationType);

        if (resourceAttribute.IsCreateDisabled is false)
        {
            Type createResourceRequestType = typeof(CreateResourceRequest<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
            Type createResourceResponseType = typeof(CreateResourceResponse<>).MakeGenericType(aggregateRootLayout.AggregateRootType);
            Type createResourceRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(createResourceRequestType, createResourceResponseType);
            Type createResourceRequestHandlerImplementationType = typeof(CreateResourceRequestProcessor<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);

            serviceCollection.AddTransient(createResourceRequestHandlerInterfaceType, createResourceRequestHandlerImplementationType);
        }

        if (resourceAttribute.IsUpdateDisabled is false)
        {
            Type updateResourceRequestType = typeof(UpdateResourceRequest<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
            Type updateResourceResponseType = typeof(UpdateResourceResponse<>).MakeGenericType(aggregateRootLayout.AggregateRootType);
            Type updateResourceRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(updateResourceRequestType, updateResourceResponseType);
            Type updateResourceRequestHandlerImplementationType = typeof(UpdateResourceRequestProcessor<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);

            serviceCollection.AddTransient(updateResourceRequestHandlerInterfaceType, updateResourceRequestHandlerImplementationType);
        }

        if (resourceAttribute.IsDeleteDisabled is false)
        {
            Type deleteResourceRequestType = typeof(DeleteResourceRequest<>).MakeGenericType(aggregateRootLayout.AggregateRootType);
            Type deleteResourceResponseType = typeof(DeleteResourceResponse);
            Type deleteResourceRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(deleteResourceRequestType, deleteResourceResponseType);
            Type deleteResourceRequestHandlerImplementationType = typeof(DeleteResourceRequestProcessor<>).MakeGenericType(aggregateRootLayout.AggregateRootType);

            serviceCollection.AddTransient(deleteResourceRequestHandlerInterfaceType, deleteResourceRequestHandlerImplementationType);
        }

        return serviceCollection;
    }

    private static IServiceCollection RegisterResourceMappers(this IServiceCollection serviceCollection, IResourceDirectory.AggregateRootLayout aggregateRootLayout)
    {
        Type resourceMapperInterfaceType = typeof(IResourceMapper<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
        Type resourceMapperImplementationType = typeof(DefaultResourceMapper<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);

        serviceCollection.AddScoped(resourceMapperInterfaceType, resourceMapperImplementationType);
        serviceCollection.AddScoped(typeof(IResourceMapper), resourceMapperImplementationType);
        
        return serviceCollection;
    }
    
    private static IServiceCollection RegisterChildResourceMappers(this IServiceCollection serviceCollection, IResourceDirectory.AggregateMemberLayout aggregateMemberLayout)
    {
        Type childResourceMapperInterfaceType = typeof(IChildResourceMapper<,>).MakeGenericType(aggregateMemberLayout.AggregateMemberType, aggregateMemberLayout.ChildResourceType);
        Type childResourceMapperImplementationType = typeof(DefaultChildResourceMapper<,>).MakeGenericType(aggregateMemberLayout.AggregateMemberType, aggregateMemberLayout.ChildResourceType);

        serviceCollection.AddScoped(childResourceMapperInterfaceType, childResourceMapperImplementationType);
        serviceCollection.AddScoped(typeof(IChildResourceMapper), childResourceMapperImplementationType);

        return serviceCollection;
    }

    private static IServiceCollection RegisterApplicationDependenciesFrom(this IServiceCollection serviceCollection, params Assembly[] assemblies)
    {
        serviceCollection.RegisterAllImplementationsOf(typeof(IApplicationService), serviceCollection.AddScoped, assemblies);
        
        return serviceCollection;
    }

    private static IServiceCollection RegisterDomainDependenciesFrom(this IServiceCollection serviceCollection, params Assembly[] assemblies)
    {
        serviceCollection.RegisterAllImplementationsOf(typeof(IDomainService), serviceCollection.AddScoped, assemblies);
        serviceCollection.RegisterAllImplementationsOf(typeof(IDomainQuery), serviceCollection.AddScoped, assemblies);
        serviceCollection.RegisterAllImplementationsOf(typeof(IDomainCommand), serviceCollection.AddScoped, assemblies);
        serviceCollection.RegisterAllImplementationsOf(typeof(IAggregateRepository<>), serviceCollection.AddScoped, assemblies);
        serviceCollection.RegisterAllImplementationsOf(typeof(IAggregateCreator<,>), serviceCollection.AddScoped, assemblies);
        serviceCollection.RegisterAllImplementationsOf(typeof(IAggregateUpdater<,>), serviceCollection.AddScoped, assemblies);

        return serviceCollection;
    }

    private static IServiceCollection WithCorsPolicy(this IServiceCollection serviceCollection, Action<CorsPolicyBuilder> configurePolicy)
    {
        serviceCollection.AddCors(corsOptions =>
        {
            corsOptions.AddPolicy(Cors.PolicyName, configurePolicy);
        });

        return serviceCollection;
    }

    private static IServiceCollection WithUsers<TUser>(this IServiceCollection serviceCollection)
        where TUser : RestEasyUser
    {
        serviceCollection.AddScoped<UserMiddleware<TUser>>();
        serviceCollection.AddScoped<IUserContextAccessor<TUser>, UserContextAccessor<TUser>>();
        serviceCollection.AddScoped<IUserContextFactory<TUser>, UserContextFactory<TUser>>();
        serviceCollection.AddScoped<IUserIdResolver, UserIdResolver>();

        serviceCollection.AddScoped<IUserIdResolutionStrategy, ClaimsPrincipalUserIdResolutionStrategy>();

        return serviceCollection;
    }

    private static IServiceCollection WithTenants<TUser, TTenant>(this IServiceCollection serviceCollection)
        where TUser : RestEasyUser
        where TTenant : RestEasyTenant
    {
        serviceCollection.AddScoped<TenantMiddleware<TUser, TTenant>>();
        serviceCollection.AddScoped<ITenantContextAccessor<TTenant>, TenantContextAccessor<TTenant>>();
        serviceCollection.AddScoped<ITenantContextFactory<TTenant>, TenantContextFactory<TTenant>>();
        serviceCollection.AddScoped<ITenantIdResolver, TenantIdResolver>();

        serviceCollection.AddScoped<ITenantIdResolutionStrategy, HeaderTenantIdResolutionStrategy>();
        serviceCollection.AddScoped<ITenantIdResolutionStrategy, QueryStringTenantIdResolutionStrategy>();

        return serviceCollection;
    }

    private static IServiceCollection WithProducts<TUser, TProduct>(this IServiceCollection serviceCollection)
        where TUser : RestEasyUser
        where TProduct : RestEasyProduct
    {
        serviceCollection.AddScoped<ProductMiddleware<TUser, TProduct>>();
        serviceCollection.AddScoped<IProductContextAccessor<TProduct>, ProductContextAccessor<TProduct>>();
        serviceCollection.AddScoped<IProductContextFactory<TProduct>, ProductContextFactory<TProduct>>();
        serviceCollection.AddScoped<IProductIdResolver, ProductIdResolver>();

        serviceCollection.AddScoped<IProductIdResolutionStrategy, HeaderProductIdResolutionStrategy>();
        serviceCollection.AddScoped<IProductIdResolutionStrategy, QueryStringProductIdResolutionStrategy>();

        return serviceCollection;
    }
    
    private static IServiceCollection WithControllers(this IServiceCollection serviceCollection, RestEasyOptions restEasyOptions, IRouteNameProvider routeNameProvider, IResourceDirectory resourceDirectory)
    {
        IMvcBuilder mvcBuilder = serviceCollection.AddControllers(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerModelConvention(routeNameProvider));

                mvcOptions.ConfigureForRestEasy();
                
                restEasyOptions.MvcOptionsConfigurationAction?.Invoke(mvcOptions);
            })
            .ConfigureApplicationPartManager(applicationPartManager =>
            {
                applicationPartManager.FeatureProviders.Add(new ControllerFeatureProvider(resourceDirectory));
                
                restEasyOptions.ApplicationPartManagerConfigurationAction?.Invoke(applicationPartManager);
            })
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.ConfigureForRestEasyFramework();
                
                restEasyOptions.JsonOptionsConfigurationAction?.Invoke(jsonOptions);
            });

        restEasyOptions.MvcBuilderConfigurationAction?.Invoke(mvcBuilder);
        
        return serviceCollection;
    }
    
    public static IServiceCollection WithOpenApi(this IServiceCollection serviceCollection, Action<SwaggerGenOptions>? configurationAction)
    {
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.DocumentFilter<RestEasyDocumentFilter>();

            configurationAction?.Invoke(options);
        });

        return serviceCollection;
    }
}