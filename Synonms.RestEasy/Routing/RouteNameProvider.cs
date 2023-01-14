using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Routing;

namespace Synonms.RestEasy.Routing;

public class RouteNameProvider : IRouteNameProvider
{
    public string GetById<TAggregateRoot>()
        where TAggregateRoot : AggregateRoot<TAggregateRoot> =>
        GetById(typeof(TAggregateRoot));

    public string GetById(Type aggregateRootType) =>
        aggregateRootType.Name + nameof(GetById);
    
    public string GetAll<TAggregateRoot>()
        where TAggregateRoot : AggregateRoot<TAggregateRoot> =>
        GetAll(typeof(TAggregateRoot));

    public string GetAll(Type aggregateRootType) =>
        aggregateRootType.Name + nameof(GetAll);
}