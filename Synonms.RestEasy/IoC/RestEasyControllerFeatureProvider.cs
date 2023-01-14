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
            AddGetById(feature, aggregateLayout);
            AddGetAll(feature, aggregateLayout);
        }
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
}