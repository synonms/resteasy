using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.SharedKernel.Collections;

namespace Synonms.RestEasy.Mediation.Queries;

public class ReadResourceCollectionResponse<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    public ReadResourceCollectionResponse(PaginatedList<TResource> resourceCollection)
    {
        ResourceCollection = resourceCollection;
    }

    public PaginatedList<TResource> ResourceCollection { get; }
}