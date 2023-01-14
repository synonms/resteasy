namespace Synonms.RestEasy.Abstractions.Routing;

public interface IResourceDirectory
{
    public class AggregateLayout
    {
        public AggregateLayout(Type aggregateType, Type resourceType)
        {
            AggregateType = aggregateType;
            ResourceType = resourceType;
        }
        
        public Type AggregateType { get; set; }
        
        public Type ResourceType { get; set; }
    }

    IReadOnlyDictionary<string, AggregateLayout> GetAll();
}