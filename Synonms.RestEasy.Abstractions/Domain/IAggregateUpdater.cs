using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Server;

namespace Synonms.RestEasy.Abstractions.Domain;

public interface IAggregateUpdater<in TAggregateRoot, in TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : ServerResource<TAggregateRoot>
{
    Task<Maybe<Fault>> UpdateAsync(TAggregateRoot aggregateRoot, TResource resource, CancellationToken cancellationToken);
}