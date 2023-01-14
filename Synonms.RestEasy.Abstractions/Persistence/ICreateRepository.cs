using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Persistence;

public interface ICreateRepository<TAggregateRoot> 
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    Task<EntityId<TAggregateRoot>> CreateAsync(TAggregateRoot aggregateRoot);
}