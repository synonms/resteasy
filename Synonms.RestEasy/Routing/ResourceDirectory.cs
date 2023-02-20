using System.Reflection;
using Synonms.RestEasy.Abstractions.Attributes;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Extensions;

namespace Synonms.RestEasy.Routing;

public class ResourceDirectory : IResourceDirectory
{
    private static readonly Dictionary<string, IResourceDirectory.AggregateRootLayout> ResourceCollectionPathToAggregateRootLayout = new();
    private static List<IResourceDirectory.AggregateMemberLayout> _aggregateMemberLayouts = new();
    
    public ResourceDirectory(params Assembly[] assemblies)
    {
        Construct(assemblies);
    }

    public IReadOnlyDictionary<string, IResourceDirectory.AggregateRootLayout> GetAllRoots() =>
        ResourceCollectionPathToAggregateRootLayout;

    public IEnumerable<IResourceDirectory.AggregateMemberLayout> GetAllMembers() =>
        _aggregateMemberLayouts;

    private static void Construct(params Assembly[] assemblies)
    {
        ResourceCollectionPathToAggregateRootLayout.Clear();
        
        List<IResourceDirectory.AggregateRootLayout> aggregateRootLayouts = assemblies
            .SelectMany(assembly => assembly.GetResources())
            .Select(resourceType => new IResourceDirectory.AggregateRootLayout(resourceType.BaseType?.GetGenericArguments().FirstOrDefault() ?? typeof(object), resourceType))
            .ToList();

        foreach (IResourceDirectory.AggregateRootLayout aggregateRootLayout in aggregateRootLayouts)
        {
            RestEasyResourceAttribute? attribute = aggregateRootLayout.AggregateRootType.GetCustomAttribute<RestEasyResourceAttribute>();

            if (attribute is not null)
            {
                ResourceCollectionPathToAggregateRootLayout[attribute.CollectionPath] = aggregateRootLayout;
            }
        }
     
        _aggregateMemberLayouts = assemblies
            .SelectMany(assembly => assembly.GetChildResources())
            .Select(childResourceType => new IResourceDirectory.AggregateMemberLayout(childResourceType.BaseType?.GetGenericArguments().FirstOrDefault() ?? typeof(object), childResourceType))
            .ToList();
    }
}