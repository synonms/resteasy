namespace Synonms.RestEasy.Abstractions.Schema.Client;

public class ClientResourceCollectionDocument<TResource> : Document
    where TResource : ClientResource
{
    public ClientResourceCollectionDocument(Link selfLink, IEnumerable<TResource> resources, Pagination pagination)
        : base(selfLink)
    {
        Resources = resources;
        Pagination = pagination;
    }

    public IEnumerable<TResource> Resources { get; }

    public Pagination Pagination { get; }

    public ClientResourceCollectionDocument<TResource> WithLink(string name, Link link)
    {
        Links[name] = link;

        return this;
    }
}