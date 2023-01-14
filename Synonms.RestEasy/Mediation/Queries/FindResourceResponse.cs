using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Mediation.Queries;

public class FindResourceResponse<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    public FindResourceResponse(Maybe<TResource> outcome)
    {
        Outcome = outcome;
    }

    public Maybe<TResource> Outcome { get; }
}