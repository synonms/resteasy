using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Persistence;

public interface IDeleteRepository<TAggregateRoot> 
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    Task DeleteAsync(EntityId<TAggregateRoot> id);
}