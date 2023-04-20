using MediatR;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Mediation.Commands;

public class CreateResourceRequest<TAggregateRoot, TResource> : IRequest<CreateResourceResponse<TAggregateRoot>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public CreateResourceRequest(TResource resource, Func<TAggregateRoot, bool>? filter = null)
    {
        Resource = resource;
        Filter = filter;
    }

    public TResource Resource { get; }
    
    public Func<TAggregateRoot, bool>? Filter { get; }
}
