using Synonms.RestEasy.Core.Domain;
using MediatR;
using Synonms.RestEasy.WebApi.Pipeline;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Mediation.Queries;

public class ReadResourceCollectionRequest<TAggregateRoot, TResource> : IRequest<ReadResourceCollectionResponse<TAggregateRoot, TResource>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public ReadResourceCollectionRequest(int limit)
    {
        Limit = limit;
    }

    public int Limit { get; }
    
    public int Offset { get; init; } = 0;

    public QueryParameters QueryParameters { get; init; } = new();
    
    public IEnumerable<SortItem> SortItems { get; init; } = new List<SortItem>();
}