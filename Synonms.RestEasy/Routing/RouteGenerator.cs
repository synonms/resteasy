using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Routing;

namespace Synonms.RestEasy.Routing;

public class RouteGenerator : IRouteGenerator
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IRouteNameProvider _routeNameProvider;

    public RouteGenerator(LinkGenerator linkGenerator, IRouteNameProvider routeNameProvider)
    {
        _linkGenerator = linkGenerator;
        _routeNameProvider = routeNameProvider;
    }

    public Uri Item<TAggregateRoot>(HttpContext httpContext, EntityId<TAggregateRoot> id) 
        where TAggregateRoot : AggregateRoot<TAggregateRoot>
    {
        string routeName = _routeNameProvider.GetById<TAggregateRoot>();
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, new { id = id.Value }, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        
        return new Uri(uriString);
    }

    public Uri Item(Type aggregateRootType, HttpContext httpContext, Guid id)
    {
        string routeName = _routeNameProvider.GetById(aggregateRootType);
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, new { id }, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        
        return new Uri(uriString);
    }
    
    public Uri Collection<TAggregateRoot>(HttpContext httpContext, int offset = 0) 
        where TAggregateRoot : AggregateRoot<TAggregateRoot>
    {
        object? values = offset > 0 ? new { offset } : null;
        string routeName = _routeNameProvider.GetAll<TAggregateRoot>();
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, values, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        
        return new Uri(uriString);
    }

    public Uri Collection(Type aggregateRootType, HttpContext httpContext, int offset = 0)
    {
        object? values = offset > 0 ? new { offset } : null;
        string routeName = _routeNameProvider.GetAll(aggregateRootType);
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, values, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        
        return new Uri(uriString);
    }
    
    public Uri CreateForm<TAggregateRoot>(HttpContext httpContext)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>
    {
        string routeName = _routeNameProvider.CreateForm<TAggregateRoot>();
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, null, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        
        return new Uri(uriString);
    }

    public Uri CreateForm(Type aggregateRootType, HttpContext httpContext)
    {
        string routeName = _routeNameProvider.CreateForm(aggregateRootType);
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, null, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        
        return new Uri(uriString);
    }

    public Uri EditForm<TAggregateRoot>(HttpContext httpContext, EntityId<TAggregateRoot> id)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>
    {
        string routeName = _routeNameProvider.EditForm<TAggregateRoot>();
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, new { id }, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        
        return new Uri(uriString);
    }

    public Uri EditForm(Type aggregateRootType, HttpContext httpContext, Guid id)
    {
        string routeName = _routeNameProvider.EditForm(aggregateRootType);
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, new { id }, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        
        return new Uri(uriString);
    }
}
