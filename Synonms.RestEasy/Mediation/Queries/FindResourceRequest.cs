using MediatR;
using Microsoft.AspNetCore.Http;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Server;

namespace Synonms.RestEasy.Mediation.Queries;

public class FindResourceRequest<TAggregateRoot, TResource> : IRequest<FindResourceResponse<TAggregateRoot, TResource>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : ServerResource<TAggregateRoot>
{
    public FindResourceRequest(HttpContext httpContext, EntityId<TAggregateRoot> id, Func<TAggregateRoot, bool>? filter = null)
    {
        HttpContext = httpContext;
        Id = id;
        Filter = filter;
    }

    public HttpContext HttpContext { get; }
    
    public EntityId<TAggregateRoot> Id { get; }
    
    public Func<TAggregateRoot, bool>? Filter { get; }
}
