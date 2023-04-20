using MediatR;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Mediation.Commands;

public class UpdateResourceRequest<TAggregateRoot, TResource> : IRequest<UpdateResourceResponse<TAggregateRoot>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public UpdateResourceRequest(EntityId<TAggregateRoot> id, TResource resource, Func<TAggregateRoot, bool>? filter = null)
    {
        Id = id;
        Resource = resource;
        Filter = filter;
    }

    public EntityId<TAggregateRoot> Id { get; }
    
    public TResource Resource { get; }
    
    public Func<TAggregateRoot, bool>? Filter { get; }
}
