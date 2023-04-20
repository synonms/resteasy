namespace Synonms.RestEasy.Abstractions.Schema;

public abstract class Resource
{
    protected Resource()
    {
        Id = Guid.Empty;
        SelfLink = Link.SelfLink(new Uri("/" + Id, UriKind.Relative));
    }
    
    protected Resource(Guid id, Link selfLink)
    {
        Id = id;
        SelfLink = selfLink;
    }

    public Guid Id { get; init; }
    
    public Link SelfLink { get; init; }

    public ResourceLinks Links { get; } = new();
}
