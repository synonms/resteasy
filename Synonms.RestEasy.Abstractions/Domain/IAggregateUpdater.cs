using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Abstractions.Domain;

public interface IAggregateUpdater<in TAggregateRoot, in TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    Task<Maybe<Fault>> UpdateAsync(TAggregateRoot aggregateRoot, TResource resource, CancellationToken cancellationToken);
}