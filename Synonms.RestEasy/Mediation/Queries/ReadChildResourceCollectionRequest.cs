using MediatR;
using Microsoft.AspNetCore.Http;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Mediation.Queries;

public class ReadChildResourceCollectionRequest<TParentEntity, TAggregateRoot, TResource> : IRequest<ReadChildResourceCollectionResponse<TParentEntity, TAggregateRoot, TResource>>
    where TParentEntity : Entity<TParentEntity>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    public ReadChildResourceCollectionRequest(HttpContext httpContext, EntityId<TParentEntity> parentId, int offset, int limit, IReadOnlyDictionary<string, object>? parameters = null)
    {
        HttpContext = httpContext;
        ParentId = parentId;
        Offset = offset;
        Limit = limit;
        Parameters = parameters ?? new Dictionary<string, object>();
    }

    public HttpContext HttpContext { get; }

    public EntityId<TParentEntity> ParentId { get; }
    
    public int Offset { get; }
    
    public int Limit { get; }
    
    public IReadOnlyDictionary<string, object> Parameters { get; }
}
