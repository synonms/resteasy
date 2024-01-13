using System.Reflection;
using Synonms.RestEasy.Core.Domain.Events;
using Synonms.RestEasy.Core.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Synonms.RestEasy.WebApi.Application;
using Synonms.RestEasy.WebApi.Attributes;
using Synonms.RestEasy.WebApi.Correlation;
using Synonms.RestEasy.WebApi.Domain.Events;
using Synonms.RestEasy.WebApi.Mediation.Commands;
using Synonms.RestEasy.WebApi.Mediation.Queries;
using Synonms.RestEasy.WebApi.Routing;
using Synonms.RestEasy.WebApi.Schema.Errors;
using Synonms.RestEasy.WebApi.Schema.Forms;

namespace Synonms.RestEasy.WebApi.Startup;

public static class ServiceCollectionExtensions
{
    public static ServiceBuilder AddRestEasyFramework(this IServiceCollection serviceCollection, params Assembly[] aggregateAssemblies) =>
        serviceCollection.AddRestEasyFramework(_ => {}, aggregateAssemblies);

    public static ServiceBuilder AddRestEasyFramework(this IServiceCollection serviceCollection, Action<MvcOptions> mvcOptionsConfiguration, params Assembly[] aggregateAssemblies)
    {
        serviceCollection.AddHttpContextAccessor();

        IResourceDirectory resourceDirectory = new ResourceDirectory(aggregateAssemblies);
        serviceCollection.AddSingleton(resourceDirectory);

        IRouteNameProvider routeNameProvider = new RouteNameProvider();
        serviceCollection.AddSingleton(routeNameProvider);
        
        serviceCollection.AddSingleton<IErrorCollectionDocumentFactory, ErrorCollectionDocumentFactory>();
        serviceCollection.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        
        serviceCollection.AddScoped<CorrelationMiddleware>();

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

        serviceCollection.AddControllers(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new ControllerModelConvention(routeNameProvider));

                mvcOptions.ConfigureForRestEasy();
                
                mvcOptionsConfiguration?.Invoke(mvcOptions);
                
                // AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                //     .RequireAuthenticatedUser()
                //     .Build();
                // mvcOptions.Filters.Add(new AuthorizeFilter(policy));
            })
            .ConfigureApplicationPartManager(applicationPartManager =>
            {
                applicationPartManager.FeatureProviders.Add(new ControllerFeatureProvider(resourceDirectory));
            })
            .AddJsonOptions(jsonOptions => jsonOptions.JsonSerializerOptions.ConfigureForRestEasyFramework());
        
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(IMediator), typeof(Mediator), ServiceLifetime.Transient));
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(ISender), sp => sp.GetRequiredService<IMediator>(), ServiceLifetime.Transient));
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(IPublisher), sp => sp.GetRequiredService<IMediator>(), ServiceLifetime.Transient));

        // Replace default mappers where an explicit one is provided
        serviceCollection.RegisterAllImplementationsOf(typeof(IResourceMapper<,>), serviceCollection.AddSingleton, aggregateAssemblies);

        serviceCollection.AddScoped<IChildResourceMapperFactory, ChildResourceMapperFactory>();
        serviceCollection.AddScoped<IResourceMapperFactory, ResourceMapperFactory>();
        
        return new ServiceBuilder(serviceCollection);
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
}