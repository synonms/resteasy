using System.Linq.Expressions;
using Synonms.RestEasy.Core.Collections;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Persistence;
using Synonms.RestEasy.WebApi.Linq;
using Synonms.RestEasy.WebApi.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Synonms.Functional;
using Synonms.Functional.Extensions;

namespace Synonms.RestEasy.EntityFramework;

public abstract class AggregateRepository<TAggregateRoot> : IAggregateRepository<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    protected readonly DbContext DbContext;

    public AggregateRepository(DbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public async Task AddAsync(TAggregateRoot entity, CancellationToken cancellationToken)
    {
        EntityEntry<TAggregateRoot> _ = await DbContext.Set<TAggregateRoot>().AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken) =>
        await DbContext.Set<TAggregateRoot>().AddRangeAsync(entities, cancellationToken);

    public async Task<bool> AnyAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken) => 
        await DbContext.Set<TAggregateRoot>().AnyAsync(predicate, cancellationToken);
    
    public Task DeleteAsync(TAggregateRoot entity, CancellationToken cancellationToken) =>
        Task.FromResult(DbContext.Remove(entity));

    public async Task DeleteAsync(EntityId<TAggregateRoot> id, CancellationToken cancellationToken) =>
        await FindAsync(id, cancellationToken)
            .MatchAsync(
                async entity => await DeleteAsync(entity, cancellationToken),
                () => Task.CompletedTask);

    public async Task DeleteAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken)
    {
        List<TAggregateRoot> entities = await DbContext.Set<TAggregateRoot>().Where(predicate).ToListAsync(cancellationToken);

        DbContext.RemoveRange(entities);
    }

    public virtual async Task<Maybe<TAggregateRoot>> FindAsync(EntityId<TAggregateRoot> id, CancellationToken cancellationToken)
    {
        TAggregateRoot? aggregateRoot = await DbContext.Set<TAggregateRoot>().FindAsync(new object[] { id }, cancellationToken);
        
        return aggregateRoot ?? Maybe<TAggregateRoot>.None;
    }

    public async Task<Maybe<TAggregateRoot>> FindFirstAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken)
    {
        TAggregateRoot? aggregateRoot = await DbContext.Set<TAggregateRoot>().FirstOrDefaultAsync(predicate, cancellationToken);

        return aggregateRoot ?? Maybe<TAggregateRoot>.None;
    }

    public Task<List<TAggregateRoot>> ListAllAsync(CancellationToken cancellationToken) =>
        DbContext.Set<TAggregateRoot>().ToListAsync(cancellationToken);

    public Task<List<TAggregateRoot>> ListAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken) =>
        DbContext.Set<TAggregateRoot>().Where(predicate).ToListAsync(cancellationToken);

    public IQueryable<TAggregateRoot> Query() =>
        DbContext.Set<TAggregateRoot>().AsQueryable();
    
    public virtual IQueryable<TAggregateRoot> Query(Expression<Func<TAggregateRoot, bool>> predicate) =>
        DbContext.Set<TAggregateRoot>().Where(predicate);

    public virtual Task<PaginatedList<TAggregateRoot>> ReadAllAsync(int offset, int limit, Func<IQueryable<TAggregateRoot>, IQueryable<TAggregateRoot>> sortFunc, CancellationToken cancellationToken) =>
        Task.FromResult(PaginatedList<TAggregateRoot>.Create(sortFunc.Invoke(DbContext.Set<TAggregateRoot>()), offset, limit));

    public Task<PaginatedList<TAggregateRoot>> ReadAsync(Expression<Func<TAggregateRoot, bool>> predicate, int offset, int limit, Func<IQueryable<TAggregateRoot>, IQueryable<TAggregateRoot>> sortFunc, CancellationToken cancellationToken) =>
        Task.FromResult(PaginatedList<TAggregateRoot>.Create(sortFunc.Invoke(DbContext.Set<TAggregateRoot>()), offset, limit));

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await DbContext.SaveChangesAsync(cancellationToken);

    public Task<List<TResult>> SelectAsync<TResult>(Expression<Func<TAggregateRoot, bool>> predicate, Expression<Func<TAggregateRoot, TResult>> selector, CancellationToken cancellationToken) =>
        DbContext.Set<TAggregateRoot>().Where(predicate).Select(selector).ToListAsync(cancellationToken);

    public Task UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken)
    {
        EntityEntry<TAggregateRoot> _ = DbContext.Update(entity);
        
        return Task.CompletedTask;
    }
}