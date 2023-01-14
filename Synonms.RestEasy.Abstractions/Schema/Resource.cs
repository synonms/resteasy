using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Schema;

public abstract class Resource<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    protected Resource()
    {
        Id = EntityId<TAggregateRoot>.Uninitialised;
        SelfLink = Link.SelfLink(new Uri("/" + Id.Value, UriKind.Relative));
    }
    
    protected Resource(EntityId<TAggregateRoot> id, Link selfLink)
    {
        Id = id;
        SelfLink = selfLink;
    }
    
    public EntityId<TAggregateRoot> Id { get; set; }
    
    public Link SelfLink { get; }

    public ResourceLinks Links { get; } = new();
}