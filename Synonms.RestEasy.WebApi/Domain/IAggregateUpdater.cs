using Synonms.RestEasy.Core.Domain;
using Synonms.Functional;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Domain;

public interface IAggregateUpdater<in TAggregateRoot, in TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    Task<Maybe<Fault>> UpdateAsync(TAggregateRoot aggregateRoot, TResource resource, CancellationToken cancellationToken);
}