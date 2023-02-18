using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Endpoints;

namespace Synonms.RestEasy.IoC;

public class RestEasyControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    private readonly IResourceDirectory _resourceDirectory;

    public RestEasyControllerFeatureProvider(IResourceDirectory resourceDirectory)
    {
        _resourceDirectory = resourceDirectory;
    }
    
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        foreach ((string _, IResourceDirectory.AggregateLayout aggregateLayout) in _resourceDirectory.GetAll())
        {
            AddDelete(feature, aggregateLayout);
            AddGetById(feature, aggregateLayout);
            AddGetAll(feature, aggregateLayout);
            AddPost(feature, aggregateLayout);
            AddPut(feature, aggregateLayout);
            AddCreateForm(feature, aggregateLayout);
            AddEditForm(feature, aggregateLayout);
        }
    }

    private static void AddDelete(ControllerFeature feature, IResourceDirectory.AggregateLayout aggregateLayout)
    {
        Type deleteEndpointType = typeof(DeleteEndpoint<>).MakeGenericType(aggregateLayout.AggregateType);
                
        Console.WriteLine("Registering endpoint [{0}].", deleteEndpointType.Name);

        feature.Controllers.Add(deleteEndpointType.GetTypeInfo());
    }

    private static void AddGetById(ControllerFeature feature, IResourceDirectory.AggregateLayout aggregateLayout)
    {
        Type getByIdEndpointType = typeof(GetByIdEndpoint<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);
                
        Console.WriteLine("Registering endpoint [{0}].", getByIdEndpointType.Name);

        feature.Controllers.Add(getByIdEndpointType.GetTypeInfo());
    }
    
    private static void AddGetAll(ControllerFeature feature, IResourceDirectory.AggregateLayout aggregateLayout)
    {
        Type getAllEndpointType = typeof(GetAllEndpoint<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);
                
        Console.WriteLine("Registering endpoint [{0}].", getAllEndpointType.Name);

        feature.Controllers.Add(getAllEndpointType.GetTypeInfo());
    }

    private static void AddPost(ControllerFeature feature, IResourceDirectory.AggregateLayout aggregateLayout)
    {
        Type postEndpointType = typeof(PostEndpoint<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);
                
        Console.WriteLine("Registering endpoint [{0}].", postEndpointType.Name);

        feature.Controllers.Add(postEndpointType.GetTypeInfo());
    }
    
    private static void AddPut(ControllerFeature feature, IResourceDirectory.AggregateLayout aggregateLayout)
    {
        Type putEndpointType = typeof(PutEndpoint<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);
                
        Console.WriteLine("Registering endpoint [{0}].", putEndpointType.Name);

        feature.Controllers.Add(putEndpointType.GetTypeInfo());
    }
    
    private static void AddCreateForm(ControllerFeature feature, IResourceDirectory.AggregateLayout aggregateLayout)
    {
        Type createFormEndpointType = typeof(CreateFormEndpoint<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);
                
        Console.WriteLine("Registering endpoint [{0}].", createFormEndpointType.Name);

        feature.Controllers.Add(createFormEndpointType.GetTypeInfo());
    }
    
    private static void AddEditForm(ControllerFeature feature, IResourceDirectory.AggregateLayout aggregateLayout)
    {
        Type editFormEndpointType = typeof(EditFormEndpoint<,>).MakeGenericType(aggregateLayout.AggregateType, aggregateLayout.ResourceType);
                
        Console.WriteLine("Registering endpoint [{0}].", editFormEndpointType.Name);

        feature.Controllers.Add(editFormEndpointType.GetTypeInfo());
    }
}