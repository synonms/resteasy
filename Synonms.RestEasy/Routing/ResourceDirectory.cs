using System.Reflection;
using Synonms.RestEasy.Abstractions.Attributes;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Extensions;

namespace Synonms.RestEasy.Routing;

public class ResourceDirectory : IResourceDirectory
{
    private static readonly Dictionary<string, IResourceDirectory.AggregateLayout> ResourceCollectionPathToAggregateLayout = new();
    
    public ResourceDirectory(params Assembly[] assemblies)
    {
        Construct(assemblies);
    }

    public IReadOnlyDictionary<string, IResourceDirectory.AggregateLayout> GetAll() =>
        ResourceCollectionPathToAggregateLayout;
    
    private static void Construct(params Assembly[] assemblies)
    {
        ResourceCollectionPathToAggregateLayout.Clear();
        
        List<IResourceDirectory.AggregateLayout> aggregateLayouts = assemblies
            .SelectMany(assembly => assembly.GetResources())
            .Select(resourceType => new IResourceDirectory.AggregateLayout(resourceType.BaseType?.GetGenericArguments().FirstOrDefault() ?? typeof(object), resourceType))
            .ToList();

        foreach (IResourceDirectory.AggregateLayout aggregateLayout in aggregateLayouts)
        {
            RestEasyResourceAttribute? attribute = aggregateLayout.AggregateType.GetCustomAttribute<RestEasyResourceAttribute>();

            if (attribute is not null)
            {
                ResourceCollectionPathToAggregateLayout[attribute.CollectionPath] = aggregateLayout;
            }
        }
    }
}