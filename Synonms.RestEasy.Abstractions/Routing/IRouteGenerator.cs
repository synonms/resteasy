using Microsoft.AspNetCore.Http;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Routing;

public interface IRouteGenerator
{
    Uri Item<TAggregateRoot>(HttpContext httpContext, EntityId<TAggregateRoot> id)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>;
    
    Uri Item(Type aggregateRootType, HttpContext httpContext, Guid id);
    
    Uri Collection<TAggregateRoot>(HttpContext httpContext, int offset = 0)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>;
    
    Uri Collection(Type aggregateRootType, HttpContext httpContext, int offset = 0);

    Uri ChildCollection<TAggregateRoot, TParentEntity>(HttpContext httpContext, EntityId<TParentEntity> parentId, int offset = 0)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>
        where TParentEntity : Entity<TParentEntity>;

    Uri ChildCollection(Type aggregateRootType, HttpContext httpContext, Guid parentId, int offset = 0);
}
