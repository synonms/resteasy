using Synonms.RestEasy.Core.Domain;
using Synonms.Functional;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Mediation.Queries;

public class FindResourceResponse<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public FindResourceResponse(Maybe<TResource> outcome, EntityTag entityTag)
    {
        Outcome = outcome;
        EntityTag = entityTag;
    }

    public Maybe<TResource> Outcome { get; }
    
    public EntityTag EntityTag { get; }
}