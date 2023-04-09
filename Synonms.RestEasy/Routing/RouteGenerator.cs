using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.SharedKernel.Extensions;

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

    public Uri Item<TAggregateRoot>(HttpContext httpContext, EntityId<TAggregateRoot> id, IQueryCollection? query = null) 
        where TAggregateRoot : AggregateRoot<TAggregateRoot>
    {
        string routeName = _routeNameProvider.GetById<TAggregateRoot>();
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, new { id = id.Value }, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        string queryString = query is null ? string.Empty : "?" + string.Join('&', query.Select(x => x.Key + "=" + x.Value));
        
        return new Uri(uriString + queryString);
    }

    public Uri Item(Type aggregateRootType, HttpContext httpContext, Guid id, IQueryCollection? query = null)
    {
        string routeName = _routeNameProvider.GetById(aggregateRootType);
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, new { id }, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        string queryString = query is null ? string.Empty : "?" + string.Join('&', query.Select(x => x.Key + "=" + x.Value));
        
        return new Uri(uriString + queryString);
    }

    public Uri Collection<TAggregateRoot>(HttpContext httpContext, IQueryCollection? query = null) 
        where TAggregateRoot : AggregateRoot<TAggregateRoot>
    {
        string routeName = _routeNameProvider.GetAll<TAggregateRoot>();
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, null, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        string queryString = query is null ? string.Empty : "?" + string.Join('&', query.Select(x => x.Key + "=" + x.Value));
        
        return new Uri(uriString + queryString);
    }

    public Uri Collection(Type aggregateRootType, HttpContext httpContext, IQueryCollection? query = null)
    {
        string routeName = _routeNameProvider.GetAll(aggregateRootType);
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, null, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        string queryString = query is null ? string.Empty : "?" + string.Join('&', query.Select(x => x.Key + "=" + x.Value));
        
        return new Uri(uriString + queryString);
    }
    
    public Uri CreateForm<TAggregateRoot>(HttpContext httpContext, IQueryCollection? query = null)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>
    {
        string routeName = _routeNameProvider.CreateForm<TAggregateRoot>();
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, null, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        string queryString = query is null ? string.Empty : "?" + string.Join('&', query.Select(x => x.Key + "=" + x.Value));
        
        return new Uri(uriString + queryString);
    }

    public Uri CreateForm(Type aggregateRootType, HttpContext httpContext, IQueryCollection? query = null)
    {
        string routeName = _routeNameProvider.CreateForm(aggregateRootType);
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, null, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        string queryString = query is null ? string.Empty : "?" + string.Join('&', query.Select(x => x.Key + "=" + x.Value));
        
        return new Uri(uriString + queryString);
    }

    public Uri EditForm<TAggregateRoot>(HttpContext httpContext, EntityId<TAggregateRoot> id, IQueryCollection? query = null)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>
    {
        string routeName = _routeNameProvider.EditForm<TAggregateRoot>();
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, new { id }, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        string queryString = query is null ? string.Empty : "?" + string.Join('&', query.Select(x => x.Key + "=" + x.Value));
        
        return new Uri(uriString + queryString);
    }

    public Uri EditForm(Type aggregateRootType, HttpContext httpContext, Guid id, IQueryCollection? query = null)
    {
        string routeName = _routeNameProvider.EditForm(aggregateRootType);
        string uriString = _linkGenerator.GetUriByRouteValues(httpContext, routeName, new { id }, options: RoutingConstants.DefaultLinkOptions) ?? string.Empty;
        string queryString = query is null ? string.Empty : "?" + string.Join('&', query.Select(x => x.Key + "=" + x.Value));
        
        return new Uri(uriString + queryString);
    }
}
