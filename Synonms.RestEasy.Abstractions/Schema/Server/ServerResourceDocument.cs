using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Schema.Server;

public class ServerResourceDocument<TAggregateRoot, TResource> : Document
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : ServerResource<TAggregateRoot>
{
    public ServerResourceDocument(Link selfLink, TResource resource) 
        : base(selfLink)
    {
        Resource = resource;
    }
    
    public TResource Resource { get; }
    
    public ServerResourceDocument<TAggregateRoot, TResource> WithLink(string name, Link link)
    {
        Links[name] = link;

        return this;
    }    
    
    public ServerResourceDocument<TAggregateRoot, TResource> WithEditForm(Uri uri)
    {
        Links[IanaLinkRelations.Forms.Edit] = Link.EditFormLink(uri);

        return this;
    }
    
    public ServerResourceDocument<TAggregateRoot, TResource> WithDelete(Uri uri)
    {
        Links[LinkRelations.Delete] = new Link(uri, IanaLinkRelations.Self, IanaHttpMethods.Delete);

        return this;
    }
}