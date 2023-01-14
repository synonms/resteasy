using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Schema.Documents;

public class ResourceCollectionDocument<TAggregateRoot, TResource> : Document
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    public ResourceCollectionDocument(Link selfLink, IEnumerable<TResource> resources,
        Pagination pagination)
        : base(selfLink)
    {
        Resources = resources;
        Pagination = pagination;
    }

    public IEnumerable<TResource> Resources { get; }

    public Pagination Pagination { get; }

    public ResourceCollectionDocument<TAggregateRoot, TResource> WithLink(string name, Link link)
    {
        Links[name] = link;

        return this;
    }

    public ResourceCollectionDocument<TAggregateRoot, TResource> WithCreateForm(Uri uri)
    {
        Links[IanaLinkRelations.Forms.Create] = Link.CreateFormLink(uri);

        return this;
    }
}