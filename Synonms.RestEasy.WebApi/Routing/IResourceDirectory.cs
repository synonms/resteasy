namespace Synonms.RestEasy.WebApi.Routing;

public interface IResourceDirectory
{
    public class AggregateRootLayout
    {
        public AggregateRootLayout(Type aggregateRootType, Type resourceType)
        {
            AggregateRootType = aggregateRootType;
            ResourceType = resourceType;
        }
        
        public Type AggregateRootType { get; set; }
        
        public Type ResourceType { get; set; }
    }

    public class AggregateMemberLayout
    {
        public AggregateMemberLayout(Type aggregateMemberType, Type childResourceType)
        {
            AggregateMemberType = aggregateMemberType;
            ChildResourceType = childResourceType;
        }
        
        public Type AggregateMemberType { get; set; }
        
        public Type ChildResourceType { get; set; }
    }

    IReadOnlyDictionary<string, AggregateRootLayout> GetAllRoots();
    
    IEnumerable<AggregateMemberLayout> GetAllMembers();
}