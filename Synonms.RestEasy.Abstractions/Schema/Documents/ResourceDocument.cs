using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Schema.Documents;

public class ResourceDocument<TAggregateRoot, TResource> : Document
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    public ResourceDocument(Link selfLink, TResource resource) 
        : base(selfLink)
    {
        Resource = resource;
    }
    
    public TResource Resource { get; }
    
    public ResourceDocument<TAggregateRoot, TResource> WithLink(string name, Link link)
    {
        Links[name] = link;

        return this;
    }    
    
    public ResourceDocument<TAggregateRoot, TResource> WithEditForm(Uri uri)
    {
        Links[IanaLinkRelations.Forms.Edit] = Link.EditFormLink(uri);

        return this;
    }
    
    public ResourceDocument<TAggregateRoot, TResource> WithDelete(Uri uri)
    {
        Links[LinkRelations.Delete] = new Link(uri, IanaLinkRelations.Self, IanaHttpMethods.Delete);

        return this;
    }
}