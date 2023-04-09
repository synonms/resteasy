namespace Synonms.RestEasy.Abstractions.Schema.Client;

public class ClientResourceDocument<TResource> : Document
    where TResource : ClientResource
{
    public ClientResourceDocument(Link selfLink, TResource resource) 
        : base(selfLink)
    {
        Resource = resource;
    }
    
    public TResource Resource { get; }
    
    public ClientResourceDocument<TResource> WithLink(string name, Link link)
    {
        Links[name] = link;

        return this;
    }    
}