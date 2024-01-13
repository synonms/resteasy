using Synonms.RestEasy.Core.Domain;
using MediatR;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Mediation.Queries;

public class FindResourceRequest<TAggregateRoot, TResource> : IRequest<FindResourceResponse<TAggregateRoot, TResource>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public FindResourceRequest(EntityId<TAggregateRoot> id)
    {
        Id = id;
    }

    public EntityId<TAggregateRoot> Id { get; }
    
    public EntityTag? IfNoneMatch { get; init; }
}