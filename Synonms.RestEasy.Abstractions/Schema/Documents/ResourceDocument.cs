using Synonms.RestEasy.Abstractions.Constants;

namespace Synonms.RestEasy.Abstractions.Schema.Documents;

public class ResourceDocument<TResource> : Document
    where TResource : Resource
{
    public ResourceDocument(Link selfLink, TResource resource) 
        : base(selfLink)
    {
        Resource = resource;
    }
    
    public TResource Resource { get; }
    
    public ResourceDocument<TResource> WithLink(string name, Link link)
    {
        Links[name] = link;

        return this;
    }    
    
    public ResourceDocument<TResource> WithEditForm(Uri uri)
    {
        Links[IanaLinkRelations.Forms.Edit] = Link.EditFormLink(uri);

        return this;
    }
    
    public ResourceDocument<TResource> WithDelete(Uri uri)
    {
        Links[LinkRelations.Delete] = new Link(uri, IanaLinkRelations.Self, IanaHttpMethods.Delete);

        return this;
    }
}