using Synonms.RestEasy.Core.Constants;

namespace Synonms.RestEasy.Core.Schema.Resources;

public class ResourceCollectionDocument<TResource> : Document
    where TResource : Resource
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

    public ResourceCollectionDocument<TResource> WithLink(string name, Link link)
    {
        Links[name] = link;

        return this;
    }

    public ResourceCollectionDocument<TResource> WithCreateForm(Uri uri)
    {
        Links[IanaLinkRelations.Forms.Create] = Link.CreateFormLink(uri);

        return this;
    }
}