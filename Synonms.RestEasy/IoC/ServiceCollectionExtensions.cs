using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Application;
using Synonms.RestEasy.Extensions;
using Synonms.RestEasy.Hypermedia.Ion;
using Synonms.RestEasy.Mediation.Commands;
using Synonms.RestEasy.Mediation.Queries;
using Synonms.RestEasy.Routing;
using Synonms.RestEasy.Schema.Documents;

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
        serviceCollection.AddSingleton<IErrorCollectionDocumentFactory, ErrorCollectionDocumentFactory>();
        serviceCollection.RegisterAllImplementationsOf(typeof(ICreateRepository<>), serviceCollection.AddSingleton, aggregateAssemblies);
        serviceCollection.RegisterAllImplementationsOf(typeof(IReadRepository<>), serviceCollection.AddSingleton, aggregateAssemblies);
        serviceCollection.RegisterAllImplementationsOf(typeof(IUpdateRepository<>), serviceCollection.AddSingleton, aggregateAssemblies);
        serviceCollection.RegisterAllImplementationsOf(typeof(IDeleteRepository<>), serviceCollection.AddSingleton, aggregateAssemblies);
        serviceCollection.RegisterAllImplementationsOf(typeof(IAggregateCreator<,>), serviceCollection.AddSingleton, aggregateAssemblies);
        serviceCollection.RegisterAllImplementationsOf(typeof(IAggregateUpdater<,>), serviceCollection.AddSingleton, aggregateAssemblies);

        foreach ((string _, IResourceDirectory.AggregateLayout aggregateLayout) in resourceDirectory.GetAll())
        { 
            serviceCollection
                .RegisterRequestHandlers(aggregateLayout)
                .RegisterResourceMappers(aggregateLayout);
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

        // Replace default mappers where an explicit one is provided
        serviceCollection.RegisterAllImplementationsOf(typeof(IResourceMapper<,>), serviceCollection.AddSingleton, aggregateAssemblies);

        return new RestEasyServiceBuilder(serviceCollection);
    }
    
    private static IServiceCollection RegisterRequestHandlers(this IServiceCollection serviceCollection, IResourceDirectory.AggregateLayout aggregateLayout)
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
        
        Type createResourceRequestType = typeof(CreateResourceRequest<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);
        Type createResourceResponseType = typeof(CreateResourceResponse<>).MakeGenericType(aggregateLayout.AggregateType);
        Type createResourceRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(createResourceRequestType, createResourceResponseType);
        Type createResourceRequestHandlerImplementationType = typeof(CreateResourceRequestProcessor<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);

        Console.WriteLine("Registering service [{0}] -> [{1}]", createResourceRequestHandlerInterfaceType.Name, createResourceRequestHandlerImplementationType.Name);
        
        serviceCollection.AddTransient(createResourceRequestHandlerInterfaceType, createResourceRequestHandlerImplementationType);
        
        Type updateResourceRequestType = typeof(UpdateResourceRequest<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);
        Type updateResourceResponseType = typeof(UpdateResourceResponse<>).MakeGenericType(aggregateLayout.AggregateType);
        Type updateResourceRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(updateResourceRequestType, updateResourceResponseType);
        Type updateResourceRequestHandlerImplementationType = typeof(UpdateResourceRequestProcessor<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);

        Console.WriteLine("Registering service [{0}] -> [{1}]", updateResourceRequestHandlerInterfaceType.Name, updateResourceRequestHandlerImplementationType.Name);
        
        serviceCollection.AddTransient(updateResourceRequestHandlerInterfaceType, updateResourceRequestHandlerImplementationType);

        Type deleteResourceRequestType = typeof(DeleteResourceRequest<>).MakeGenericType(aggregateLayout.AggregateType);
        Type deleteResourceResponseType = typeof(DeleteResourceResponse);
        Type deleteResourceRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(deleteResourceRequestType, deleteResourceResponseType);
        Type deleteResourceRequestHandlerImplementationType = typeof(DeleteResourceRequestProcessor<>).MakeGenericType(aggregateLayout.AggregateType);

        Console.WriteLine("Registering service [{0}] -> [{1}]", deleteResourceRequestHandlerInterfaceType.Name, deleteResourceRequestHandlerImplementationType.Name);
        
        serviceCollection.AddTransient(deleteResourceRequestHandlerInterfaceType, deleteResourceRequestHandlerImplementationType);

        return serviceCollection;
    }

    private static IServiceCollection RegisterResourceMappers(this IServiceCollection serviceCollection, IResourceDirectory.AggregateLayout aggregateLayout)
    {
        Type resourceMapperInterfaceType = typeof(IResourceMapper<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);
        Type resourceMapperImplementationType = typeof(DefaultResourceMapper<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);

        serviceCollection.AddSingleton(resourceMapperInterfaceType, resourceMapperImplementationType);

        return serviceCollection;
    }
    
    private static IServiceCollection RegisterAllImplementationsOf(this IServiceCollection serviceCollection, Type interfaceType, Func<Type, Type, IServiceCollection> registrationFunc, params Assembly[] assemblies)
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