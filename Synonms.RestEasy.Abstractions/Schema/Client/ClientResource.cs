namespace Synonms.RestEasy.Abstractions.Schema.Client;

public abstract class ClientResource
{
    protected ClientResource()
    {
        Id = Guid.Empty;
        SelfLink = Link.SelfLink(new Uri("/" + Id, UriKind.Relative));
    }
    
    protected ClientResource(Guid id, Link selfLink)
    {
        Id = id;
        SelfLink = selfLink;
    }

    public Guid Id { get; init; }
    
    public Link SelfLink { get; init; }

    public ResourceLinks Links { get; } = new();
}