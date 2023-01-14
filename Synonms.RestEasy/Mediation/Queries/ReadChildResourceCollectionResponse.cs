using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.SharedKernel.Collections;

namespace Synonms.RestEasy.Mediation.Queries;

public class ReadChildResourceCollectionResponse<TParentEntity, TAggregateRoot, TResource>
    where TParentEntity : Entity<TParentEntity>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    public ReadChildResourceCollectionResponse(PaginatedList<TResource> resourceCollection)
    {
        ResourceCollection = resourceCollection;
    }

    public PaginatedList<TResource> ResourceCollection { get; }
}