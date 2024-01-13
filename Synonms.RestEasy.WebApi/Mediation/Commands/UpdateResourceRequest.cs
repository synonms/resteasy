using Synonms.RestEasy.Core.Domain;
using MediatR;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Mediation.Commands;

public class UpdateResourceRequest<TAggregateRoot, TResource> : IRequest<UpdateResourceResponse<TAggregateRoot>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public UpdateResourceRequest(EntityId<TAggregateRoot> id, TResource resource)
    {
        Id = id;
        Resource = resource;
    }

    public EntityId<TAggregateRoot> Id { get; }
    
    public TResource Resource { get; }
    
    public EntityTag? IfMatch { get; init; }
}