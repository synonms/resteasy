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
    
    Uri CreateForm<TAggregateRoot>(HttpContext httpContext)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>;
    
    Uri CreateForm(Type aggregateRootType, HttpContext httpContext);
    
    Uri EditForm<TAggregateRoot>(HttpContext httpContext, EntityId<TAggregateRoot> id)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>;
    
    Uri EditForm(Type aggregateRootType, HttpContext httpContext, Guid id);
}
