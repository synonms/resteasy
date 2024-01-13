using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.WebApi.Pipeline;

namespace Synonms.RestEasy.WebApi.Routing;

public interface IRouteGenerator
{
    Uri Item<TAggregateRoot>(EntityId<TAggregateRoot> id, QueryParameters? queryParameters = null)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>;
    
    Uri Item(Type aggregateRootType, Guid id, QueryParameters? queryParameters = null);

    Uri Collection<TAggregateRoot>(QueryParameters? queryParameters = null)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>;
    
    Uri Collection(Type aggregateRootType, QueryParameters? queryParameters = null);
    
    Uri CreateForm<TAggregateRoot>(QueryParameters? queryParameters = null)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>;
    
    Uri CreateForm(Type aggregateRootType, QueryParameters? queryParameters = null);
    
    Uri EditForm<TAggregateRoot>(EntityId<TAggregateRoot> id, QueryParameters? queryParameters = null)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>;
    
    Uri EditForm(Type aggregateRootType, Guid id, QueryParameters? queryParameters = null);
}