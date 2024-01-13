using System.Linq.Expressions;
using Synonms.Functional;
using Synonms.RestEasy.Core.Collections;
using Synonms.RestEasy.Core.Domain;

namespace Synonms.RestEasy.Core.Persistence;

public interface IAggregateRepository
{
}

public interface IAggregateRepository<TAggregateRoot> : IAggregateRepository
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    Task AddAsync(TAggregateRoot entity, CancellationToken cancellationToken);

    Task AddRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken);
        
    Task<bool> AnyAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken);

    Task DeleteAsync(TAggregateRoot entity, CancellationToken cancellationToken);

    Task DeleteAsync(EntityId<TAggregateRoot> id, CancellationToken cancellationToken);

    Task DeleteAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken);

    Task<Maybe<TAggregateRoot>> FindAsync(EntityId<TAggregateRoot> id, CancellationToken cancellationToken);

    Task<Maybe<TAggregateRoot>> FindFirstAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken);

    Task<List<TAggregateRoot>> ListAllAsync(CancellationToken cancellationToken);

    Task<List<TAggregateRoot>> ListAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken);

    IQueryable<TAggregateRoot> Query();

    IQueryable<TAggregateRoot> Query(Expression<Func<TAggregateRoot, bool>> predicate);

    Task<PaginatedList<TAggregateRoot>> ReadAllAsync(int offset, int limit, Func<IQueryable<TAggregateRoot>, IQueryable<TAggregateRoot>> sortFunc, CancellationToken cancellationToken);

    Task<PaginatedList<TAggregateRoot>> ReadAsync(Expression<Func<TAggregateRoot, bool>> predicate, int offset, int limit, Func<IQueryable<TAggregateRoot>, IQueryable<TAggregateRoot>> sortFunc, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<List<TResult>> SelectAsync<TResult>(Expression<Func<TAggregateRoot, bool>> predicate, Expression<Func<TAggregateRoot, TResult>> selector,
        CancellationToken cancellationToken);
        
    Task UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken);
}