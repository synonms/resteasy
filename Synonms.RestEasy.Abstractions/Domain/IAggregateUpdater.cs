using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Abstractions.Domain;

public interface IAggregateUpdater<in TAggregateRoot, in TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    Maybe<Fault> Update(TAggregateRoot aggregateRoot, TResource resource);
}