using Synonms.RestEasy.Core.Collections;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Mediation.Queries;

public class ReadResourceCollectionResponse<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public ReadResourceCollectionResponse(PaginatedList<TResource> resourceCollection)
    {
        ResourceCollection = resourceCollection;
    }

    public PaginatedList<TResource> ResourceCollection { get; }
}