using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Server;
using Synonms.RestEasy.Application;
using Synonms.RestEasy.Domain;
using Synonms.RestEasy.Extensions;
using Synonms.RestEasy.Mediation.Commands;
using Synonms.RestEasy.Mediation.Queries;
using Synonms.RestEasy.Routing;
using Synonms.RestEasy.Schema.Documents;
using Synonms.RestEasy.SharedKernel.Extensions;

namespace Synonms.RestEasy.IoC;

public static class ServiceCollectionExtensions
{
    public static RestEasyServiceBuilder AddRestEasy(this IServiceCollection serviceCollection, params Assembly[] aggregateAssemblies) =>
        serviceCollection.AddRestEasy(_ => {}, aggregateAssemblies);

    public static RestEasyServiceBuilder AddRestEasy(this IServiceCollection serviceCollection, Action<MvcOptions> mvcOptionsConfiguration, params Assembly[] aggregateAssemblies)
    {
        IResourceDirectory resourceDirectory = new ResourceDirectory(aggregateAssemblies);
        serviceCollection.AddSingleton(resourceDirectory);

        IRouteNameProvider routeNameProvider = new RouteNameProvider();
        serviceCollection.AddSingleton(routeNameProvider);

        serviceCollection.AddSingleton<IRouteGenerator, RouteGenerator>();
        serviceCollection.AddSingleton(typeof(ICreateFormDocumentFactory<,>), typeof(CreateFormDocumentFactory<,>));
        serviceCollection.AddSingleton(typeof(IEditFormDocumentFactory<,>), typeof(EditFormDocumentFactory<,>));
        serviceCollection.AddSingleton<IErrorCollectionDocumentFactory, ErrorCollectionDocumentFactory>();
        serviceCollection.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        serviceCollection.RegisterAllImplementationsOf(typeof(IAggregateCreator<,>), serviceCollection.AddSingleton, aggregateAssemblies);
        serviceCollection.RegisterAllImplementationsOf(typeof(IAggregateUpdater<,>), serviceCollection.AddSingleton, aggregateAssemblies);

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
                mvcOptions.Conventions.Add(new RestEasyControllerModelConvention(routeNameProvider));

                mvcOptions.ConfigureForRestEasy();
                
                mvcOptionsConfiguration?.Invoke(mvcOptions);
                
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

        serviceCollection.AddSingleton<IChildResourceMapperFactory, ChildResourceMapperFactory>();
        
        return new RestEasyServiceBuilder(serviceCollection);
    }
    
    private static IServiceCollection RegisterRequestHandlers(this IServiceCollection serviceCollection, IResourceDirectory.AggregateRootLayout aggregateRootLayout)
    {
        Console.WriteLine("Processing AggregateRoot [{0}]...", aggregateRootLayout.AggregateRootType.Name);
        
        Type findResourceRequestType = typeof(FindResourceRequest<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
        Type findResourceResponseType = typeof(FindResourceResponse<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
        Type findResourceRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(findResourceRequestType, findResourceResponseType);
        Type findResourceRequestHandlerImplementationType = typeof(FindResourceRequestProcessor<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);

        Console.WriteLine("Registering service [{0}] -> [{1}]", findResourceRequestHandlerInterfaceType.Name, findResourceRequestHandlerImplementationType.Name);
        
        serviceCollection.AddTransient(findResourceRequestHandlerInterfaceType, findResourceRequestHandlerImplementationType);
        
        Type readResourceCollectionRequestType = typeof(ReadResourceCollectionRequest<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
        Type readResourceCollectionResponseType = typeof(ReadResourceCollectionResponse<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
        Type readResourceCollectionRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(readResourceCollectionRequestType, readResourceCollectionResponseType);
        Type readResourceCollectionRequestHandlerImplementationType = typeof(ReadResourceCollectionRequestProcessor<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);

        Console.WriteLine("Registering service [{0}] -> [{1}]", readResourceCollectionRequestHandlerInterfaceType.Name, readResourceCollectionRequestHandlerImplementationType.Name);
        
        serviceCollection.AddTransient(readResourceCollectionRequestHandlerInterfaceType, readResourceCollectionRequestHandlerImplementationType);
        
        Type createResourceRequestType = typeof(CreateResourceRequest<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
        Type createResourceResponseType = typeof(CreateResourceResponse<>).MakeGenericType(aggregateRootLayout.AggregateRootType);
        Type createResourceRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(createResourceRequestType, createResourceResponseType);
        Type createResourceRequestHandlerImplementationType = typeof(CreateResourceRequestProcessor<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);

        Console.WriteLine("Registering service [{0}] -> [{1}]", createResourceRequestHandlerInterfaceType.Name, createResourceRequestHandlerImplementationType.Name);
        
        serviceCollection.AddTransient(createResourceRequestHandlerInterfaceType, createResourceRequestHandlerImplementationType);
        
        Type updateResourceRequestType = typeof(UpdateResourceRequest<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
        Type updateResourceResponseType = typeof(UpdateResourceResponse<>).MakeGenericType(aggregateRootLayout.AggregateRootType);
        Type updateResourceRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(updateResourceRequestType, updateResourceResponseType);
        Type updateResourceRequestHandlerImplementationType = typeof(UpdateResourceRequestProcessor<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);

        Console.WriteLine("Registering service [{0}] -> [{1}]", updateResourceRequestHandlerInterfaceType.Name, updateResourceRequestHandlerImplementationType.Name);
        
        serviceCollection.AddTransient(updateResourceRequestHandlerInterfaceType, updateResourceRequestHandlerImplementationType);

        Type deleteResourceRequestType = typeof(DeleteResourceRequest<>).MakeGenericType(aggregateRootLayout.AggregateRootType);
        Type deleteResourceResponseType = typeof(DeleteResourceResponse);
        Type deleteResourceRequestHandlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(deleteResourceRequestType, deleteResourceResponseType);
        Type deleteResourceRequestHandlerImplementationType = typeof(DeleteResourceRequestProcessor<>).MakeGenericType(aggregateRootLayout.AggregateRootType);

        Console.WriteLine("Registering service [{0}] -> [{1}]", deleteResourceRequestHandlerInterfaceType.Name, deleteResourceRequestHandlerImplementationType.Name);
        
        serviceCollection.AddTransient(deleteResourceRequestHandlerInterfaceType, deleteResourceRequestHandlerImplementationType);

        return serviceCollection;
    }

    private static IServiceCollection RegisterResourceMappers(this IServiceCollection serviceCollection, IResourceDirectory.AggregateRootLayout aggregateRootLayout)
    {
        Type resourceMapperInterfaceType = typeof(IResourceMapper<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);
        Type resourceMapperImplementationType = typeof(DefaultResourceMapper<,>).MakeGenericType(aggregateRootLayout.AggregateRootType, aggregateRootLayout.ResourceType);

        serviceCollection.AddSingleton(resourceMapperInterfaceType, resourceMapperImplementationType);

        return serviceCollection;
    }
    
    private static IServiceCollection RegisterChildResourceMappers(this IServiceCollection serviceCollection, IResourceDirectory.AggregateMemberLayout aggregateMemberLayout)
    {
        Type childResourceMapperInterfaceType = typeof(IChildResourceMapper<,>).MakeGenericType(aggregateMemberLayout.AggregateMemberType, aggregateMemberLayout.ChildResourceType);
        Type childResourceMapperImplementationType = typeof(DefaultChildResourceMapper<,>).MakeGenericType(aggregateMemberLayout.AggregateMemberType, aggregateMemberLayout.ChildResourceType);

        serviceCollection.AddSingleton(childResourceMapperInterfaceType, childResourceMapperImplementationType);
        serviceCollection.AddSingleton(typeof(IChildResourceMapper), childResourceMapperImplementationType);

        return serviceCollection;
    }
}