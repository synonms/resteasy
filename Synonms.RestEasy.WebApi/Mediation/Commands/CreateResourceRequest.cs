using Synonms.RestEasy.Core.Domain;
using MediatR;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Mediation.Commands;

public class CreateResourceRequest<TAggregateRoot, TResource> : IRequest<CreateResourceResponse<TAggregateRoot>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public CreateResourceRequest(TResource resource)
    {
        Resource = resource;
    }

    public TResource Resource { get; }
}