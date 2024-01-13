using Synonms.RestEasy.WebApi.Constants;

namespace Synonms.RestEasy.WebApi.Schema;

public abstract class Document
{
    private Dictionary<string, Link> _links = new ();
    
    protected Document(Link selfLink)
    {
        _links[IanaLinkRelations.Self] = selfLink;
    }

    public Dictionary<string, Link> Links => _links;
}