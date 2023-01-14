using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Persistence;

public interface IUpdateRepository<in TAggregateRoot> 
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    Task UpdateAsync(TAggregateRoot aggregateRoot);
}