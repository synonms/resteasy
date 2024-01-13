using System.Reflection;
using Synonms.RestEasy.WebApi.Runtime;
using Synonms.RestEasy.WebApi.Attributes;

namespace Synonms.RestEasy.WebApi.Routing;

public class ResourceDirectory : IResourceDirectory
{
    private static readonly Dictionary<string, IResourceDirectory.AggregateRootLayout> ResourceCollectionPathToAggregateRootLayout = new();
    private static readonly List<IResourceDirectory.AggregateMemberLayout> AggregateMemberLayouts = new();
    
    public ResourceDirectory(params Assembly[] assemblies)
    {
        Construct(assemblies);
    }

    public IReadOnlyDictionary<string, IResourceDirectory.AggregateRootLayout> GetAllRoots() =>
        ResourceCollectionPathToAggregateRootLayout;

    public IEnumerable<IResourceDirectory.AggregateMemberLayout> GetAllMembers() =>
        AggregateMemberLayouts;

    private static void Construct(params Assembly[] assemblies)
    {
        ResourceCollectionPathToAggregateRootLayout.Clear();
        AggregateMemberLayouts.Clear();
        
        foreach (Type aggregateRootType in assemblies.SelectMany(assembly => assembly.GetAggregateRoots()))
        {
            RestEasyResourceAttribute? attribute = aggregateRootType.GetCustomAttribute<RestEasyResourceAttribute>();

            if (attribute is not null)
            {
                ResourceCollectionPathToAggregateRootLayout[attribute.CollectionPath] = new IResourceDirectory.AggregateRootLayout(aggregateRootType, attribute.ResourceType);
            }
        }

        foreach (Type aggregateMemberType in assemblies.SelectMany(assembly => assembly.GetAggregateMembers()))
        {
            RestEasyChildResourceAttribute? attribute = aggregateMemberType.GetCustomAttribute<RestEasyChildResourceAttribute>();

            if (attribute is not null)
            {
                AggregateMemberLayouts.Add(new IResourceDirectory.AggregateMemberLayout(aggregateMemberType, attribute.ChildResourceType));
            }
        }
    }
}