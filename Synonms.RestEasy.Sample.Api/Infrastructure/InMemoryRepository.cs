using System.Linq.Expressions;
using Synonms.Functional;
using Synonms.RestEasy.Core.Collections;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Persistence;

namespace Synonms.RestEasy.Sample.Api.Infrastructure;

public abstract class InMemoryRepository<TAggregateRoot> : IAggregateRepository<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    private readonly List<TAggregateRoot> _aggregateRoots;

    protected InMemoryRepository(IEnumerable<TAggregateRoot> aggregateRoots)
    {
        _aggregateRoots = aggregateRoots.ToList();
    }
    
    public Task AddAsync(TAggregateRoot entity, CancellationToken cancellationToken)
    {
        _aggregateRoots.Add(entity);
        
        return Task.CompletedTask;
    }

    public Task AddRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken)
    {
        _aggregateRoots.AddRange(entities);
        
        return Task.CompletedTask;
    }

    public Task<bool> AnyAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken) =>
        Task.FromResult(_aggregateRoots.Any(predicate.Compile()));

    public Task DeleteAsync(TAggregateRoot entity, CancellationToken cancellationToken)
    {
        _aggregateRoots.Remove(entity);
        
        return Task.CompletedTask;
    }

    public Task DeleteAsync(EntityId<TAggregateRoot> id, CancellationToken cancellationToken)
    {
        _aggregateRoots.RemoveAll(x => x.Id == id);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Maybe<TAggregateRoot>> FindAsync(EntityId<TAggregateRoot> id, CancellationToken cancellationToken)
    {
        Maybe<TAggregateRoot> aggregateRoot = _aggregateRoots.FirstOrDefault(x => x.Id == id) ?? Maybe<TAggregateRoot>.None;
        
        return Task.FromResult(aggregateRoot);
    }

    public Task<Maybe<TAggregateRoot>> FindFirstAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<TAggregateRoot>> ListAllAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<TAggregateRoot>> ListAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public IQueryable<TAggregateRoot> Query()
    {
        throw new NotImplementedException();
    }

    public IQueryable<TAggregateRoot> Query(Expression<Func<TAggregateRoot, bool>> predicate) =>
        _aggregateRoots.Where(predicate.Compile()).AsQueryable();

    public Task<PaginatedList<TAggregateRoot>> ReadAllAsync(int offset, int limit, Func<IQueryable<TAggregateRoot>, IQueryable<TAggregateRoot>> sortFunc, CancellationToken cancellationToken) =>
        Task.FromResult(PaginatedList<TAggregateRoot>.Create(sortFunc.Invoke(_aggregateRoots.AsQueryable()), offset, limit));

    public Task<PaginatedList<TAggregateRoot>> ReadAsync(Expression<Func<TAggregateRoot, bool>> predicate, int offset, int limit, Func<IQueryable<TAggregateRoot>, IQueryable<TAggregateRoot>> sortFunc, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public Task<List<TResult>> SelectAsync<TResult>(Expression<Func<TAggregateRoot, bool>> predicate, Expression<Func<TAggregateRoot, TResult>> selector, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken)
    {
        _aggregateRoots.RemoveAll(x => x.Id == entity.Id);
        _aggregateRoots.Add(entity);
        
        return Task.CompletedTask;
    }
}