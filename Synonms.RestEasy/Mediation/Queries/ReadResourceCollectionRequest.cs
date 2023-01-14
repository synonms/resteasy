using MediatR;
using Microsoft.AspNetCore.Http;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Mediation.Queries;

public class ReadResourceCollectionRequest<TAggregateRoot, TResource> : IRequest<ReadResourceCollectionResponse<TAggregateRoot, TResource>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    public ReadResourceCollectionRequest(HttpContext httpContext, int offset, int limit, IReadOnlyDictionary<string, object>? parameters = null)
    {
        Offset = offset;
        Limit = limit;
        HttpContext = httpContext;
        Parameters = parameters ?? new Dictionary<string, object>();
    }

    public HttpContext HttpContext { get; }

    public int Offset { get; }
    
    public int Limit { get; }
    
    public IReadOnlyDictionary<string, object> Parameters { get; }
}
