using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Schema.Server;

public abstract class ServerResource<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    protected ServerResource()
    {
        Id = EntityId<TAggregateRoot>.Uninitialised;
        SelfLink = Link.SelfLink(new Uri("/" + Id.Value, UriKind.Relative));
    }
    
    protected ServerResource(EntityId<TAggregateRoot> id, Link selfLink)
    {
        Id = id;
        SelfLink = selfLink;
    }

    public EntityId<TAggregateRoot> Id { get; init; }
    
    public Link SelfLink { get; init; }

    public ResourceLinks Links { get; } = new();
}
