using Microsoft.AspNetCore.Http;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Routing;

public interface IRouteGenerator
{
    Uri Item<TAggregateRoot>(HttpContext httpContext, EntityId<TAggregateRoot> id, IQueryCollection? query = null)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>;
    
    Uri Item(Type aggregateRootType, HttpContext httpContext, Guid id, IQueryCollection? query = null);

    Uri Collection<TAggregateRoot>(HttpContext httpContext, IQueryCollection? query = null)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>;
    
    Uri Collection(Type aggregateRootType, HttpContext httpContext, IQueryCollection? query = null);
    
    Uri CreateForm<TAggregateRoot>(HttpContext httpContext, IQueryCollection? query = null)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>;
    
    Uri CreateForm(Type aggregateRootType, HttpContext httpContext, IQueryCollection? query = null);
    
    Uri EditForm<TAggregateRoot>(HttpContext httpContext, EntityId<TAggregateRoot> id, IQueryCollection? query = null)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>;
    
    Uri EditForm(Type aggregateRootType, HttpContext httpContext, Guid id, IQueryCollection? query = null);
}
