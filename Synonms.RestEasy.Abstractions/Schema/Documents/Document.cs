using Synonms.RestEasy.Abstractions.Constants;

namespace Synonms.RestEasy.Abstractions.Schema.Documents;

public abstract class Document
{
    private Dictionary<string, Link> _links = new ();
    
    protected Document(Link selfLink)
    {
        _links[IanaLinkRelations.Self] = selfLink;
    }

    public Dictionary<string, Link> Links => _links;
}