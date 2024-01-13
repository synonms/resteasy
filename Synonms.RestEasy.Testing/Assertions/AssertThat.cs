using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Schema;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.Testing.Assertions;

public static class AssertThat
{
    public static TResource Resource<TResource>(TResource resource) 
        where TResource : Resource => 
        resource;
        
    public static Dictionary<string, Link> Links(Dictionary<string, Link> links) => 
        links;

    public static IReadOnlyDictionary<string, Link> Links(IReadOnlyDictionary<string, Link> links) => 
        links;

    public static Dictionary<string, Link> LinksFromPagination(Pagination pagination)
    {
        Dictionary<string, Link> actualLinks = new Dictionary<string, Link>()
        {
            [IanaLinkRelations.Pagination.First] = pagination.First,
            [IanaLinkRelations.Pagination.Last] = pagination.Last
        };

        if (pagination.Previous is not null)
        {
            actualLinks[IanaLinkRelations.Pagination.Previous] = pagination.Previous;
        }
        
        if (pagination.Next is not null)
        {
            actualLinks[IanaLinkRelations.Pagination.Next] = pagination.Next;
        }

        return actualLinks;
    }
}