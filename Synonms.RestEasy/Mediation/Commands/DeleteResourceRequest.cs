using MediatR;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Mediation.Commands;

public class DeleteResourceRequest<TAggregateRoot> : IRequest<DeleteResourceResponse>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    public DeleteResourceRequest(EntityId<TAggregateRoot> id, Func<TAggregateRoot, bool>? filter = null)
    {
        Id = id;
        Filter = filter;
    }

    public EntityId<TAggregateRoot> Id { get; }
    
    public Func<TAggregateRoot, bool>? Filter { get; }
}