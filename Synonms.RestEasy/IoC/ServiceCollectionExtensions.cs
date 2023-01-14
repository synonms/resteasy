using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Extensions;
using Synonms.RestEasy.Hypermedia.Ion;
using Synonms.RestEasy.Mediation.Queries;
using Synonms.RestEasy.Routing;

namespace Synonms.RestEasy.IoC;

public static class ServiceCollectionExtensions
{
    public static RestEasyServiceBuilder AddRestEasy(this IServiceCollection serviceCollection, params Assembly[] aggregateAssemblies)
    {
        IResourceDirectory resourceDirectory = new ResourceDirectory(aggregateAssemblies);
        serviceCollection.AddSingleton(resourceDirectory);

        IRouteNameProvider routeNameProvider = new RouteNameProvider();
        serviceCollection.AddSingleton(routeNameProvider);

        serviceCollection.AddSingleton<IRouteGenerator, RouteGenerator>();
        serviceCollection.RegisterAllImplementationsOf(typeof(IResourceMapper<,>), serviceCollection.AddSingleton, aggregateAssemblies);

        foreach ((string _, IResourceDirectory.AggregateLayout aggregateLayout) in resourceDirectory.GetAll())
        { 
            serviceCollection.RegisterRequestHandlers(aggregateLayout);
        }

        serviceCollection.AddControllers(mvcOptions =>
            {
                mvcOptions.Conventions.Add(new RestEasyControllerModelConvention(routeNameProvider));

                mvcOptions.ConfigureForRestEasy().WithIon();
                // AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                //     .RequireAuthenticatedUser()
                //     .Build();
                // mvcOptions.Filters.Add(new AuthorizeFilter(policy));
            })
            .ConfigureApplicationPartManager(applicationPartManager =>
            {
                applicationPartManager.FeatureProviders.Add(new RestEasyControllerFeatureProvider(resourceDirectory));
            })
            .AddJsonOptions(jsonOptions => jsonOptions.JsonSerializerOptions.ConfigureForRestEasy());
        
        serviceCollection.TryAddTransient<ServiceFactory>(p => p.GetRequiredService);
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(IMediator), typeof(Mediator), ServiceLifetime.Transient));
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(ISender), sp => sp.GetRequiredService<IMediator>(), ServiceLifetime.Transient));
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(IPublisher), sp => sp.GetRequiredService<IMediator>(), ServiceLifetime.Transient));

        return new RestEasyServiceBuilder(serviceCollection);
    }
    
    private static void RegisterRequestHandlers(this IServiceCollection serviceCollection, IResourceDirectory.AggregateLayout aggregateLayout)
    {
        Console.WriteLine("Processing AggregateRoot [{0}]...", aggregateLayout.AggregateType.Name);
        
        Type findResourceRequestType = typeof(FindResourceRequest<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);
        Type findResourceResponseType = typeof(FindResourceResponse<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);
        Type findResourceRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(findResourceRequestType, findResourceResponseType);
        Type findResourceRequestHandlerImplementationType = typeof(FindResourceRequestProcessor<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);

        Console.WriteLine("Registering service [{0}] -> [{1}]", findResourceRequestHandlerInterfaceType.Name, findResourceRequestHandlerImplementationType.Name);
        
        serviceCollection.AddTransient(findResourceRequestHandlerInterfaceType, findResourceRequestHandlerImplementationType);
        
        Type readResourceCollectionRequestType = typeof(ReadResourceCollectionRequest<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);
        Type readResourceCollectionResponseType = typeof(ReadResourceCollectionResponse<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);
        Type readResourceCollectionRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(readResourceCollectionRequestType, readResourceCollectionResponseType);
        Type readResourceCollectionRequestHandlerImplementationType = typeof(ReadResourceCollectionRequestProcessor<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);

        Console.WriteLine("Registering service [{0}] -> [{1}]", readResourceCollectionRequestHandlerInterfaceType.Name, readResourceCollectionRequestHandlerImplementationType.Name);
        
        serviceCollection.AddTransient(readResourceCollectionRequestHandlerInterfaceType, readResourceCollectionRequestHandlerImplementationType);
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