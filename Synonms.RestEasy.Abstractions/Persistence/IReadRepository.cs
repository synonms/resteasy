using System.Linq.Expressions;
using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.SharedKernel.Collections;

namespace Synonms.RestEasy.Abstractions.Persistence;

public interface IReadRepository<TAggregateRoot> 
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    Task<Maybe<TAggregateRoot>> FindAsync(EntityId<TAggregateRoot> id);
    
    Task<IQueryable<TAggregateRoot>> QueryAsync(Expression<Func<TAggregateRoot, bool>> predicate);

    Task<PaginatedList<TAggregateRoot>> ReadAsync(int offset, int limit);
}