using System.Linq.Expressions;
using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.SharedKernel.Collections;

namespace Synonms.RestEasy.Sample.Api.Hacks;

public class InMemoryRepository<TAggregateRoot> : IReadRepository<TAggregateRoot>, ICreateRepository<TAggregateRoot>, IUpdateRepository<TAggregateRoot>, IDeleteRepository<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    private readonly List<TAggregateRoot> _aggregateRoots;

    public InMemoryRepository(IEnumerable<TAggregateRoot> aggregateRoots)
    {
        _aggregateRoots = aggregateRoots.ToList();
    }
    
    public Task<Maybe<TAggregateRoot>> FindAsync(EntityId<TAggregateRoot> id)
    {
        TAggregateRoot? person = _aggregateRoots.SingleOrDefault(x => x.Id == id);
        Maybe<TAggregateRoot> outcome = person is null ? Maybe<TAggregateRoot>.None : Maybe<TAggregateRoot>.Some(person);

        return Task.FromResult(outcome);
    }

    public Task<IQueryable<TAggregateRoot>> QueryAsync(Expression<Func<TAggregateRoot, bool>> predicate) =>
        Task.FromResult(_aggregateRoots.Where(predicate.Compile()).AsQueryable());

    public Task<PaginatedList<TAggregateRoot>> ReadAsync(int offset, int limit) =>
        Task.FromResult(PaginatedList<TAggregateRoot>.Create(_aggregateRoots.AsQueryable(), offset, limit));

    public Task<EntityId<TAggregateRoot>> CreateAsync(TAggregateRoot aggregateRoot)
    {
        _aggregateRoots.Add(aggregateRoot);

        return Task.FromResult(aggregateRoot.Id);
    }

    public Task UpdateAsync(TAggregateRoot aggregateRoot)
    {
        _aggregateRoots.RemoveAll(x => x.Id == aggregateRoot.Id);
        _aggregateRoots.Add(aggregateRoot);
        
        return Task.CompletedTask;
    }

    public Task DeleteAsync(EntityId<TAggregateRoot> id)
    {
        _aggregateRoots.RemoveAll(x => x.Id == id);
        
        return Task.CompletedTask;
    }
}