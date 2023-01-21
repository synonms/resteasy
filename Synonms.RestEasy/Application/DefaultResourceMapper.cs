using System.Reflection;
using Microsoft.AspNetCore.Http;
using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Extensions;
using Synonms.RestEasy.SharedKernel.Extensions;

namespace Synonms.RestEasy.Application;

public class DefaultResourceMapper<TAggregateRoot, TResource> : IResourceMapper<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>, new() 
{
    private readonly IRouteGenerator _routeGenerator;
    
    public DefaultResourceMapper(IRouteGenerator routeGenerator)
    {
        _routeGenerator = routeGenerator;
    }
    
    public TResource Map(HttpContext httpContext, TAggregateRoot aggregateRoot)
    {
        Uri selfUri = _routeGenerator.Item(httpContext, aggregateRoot.Id);
        Link selfLink = Link.SelfLink(selfUri);

        TResource resource = new()
        {
            Id = aggregateRoot.Id,
            SelfLink = selfLink
        };

        foreach (PropertyInfo resourcePropertyInfo in typeof(TResource).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (resourcePropertyInfo.Name.Equals("Id") || resourcePropertyInfo.Name.Equals("SelfLink") || resourcePropertyInfo.Name.Equals("Links"))
            {
                continue;
            }
            
            PropertyInfo? aggregateRootPropertyInfo = typeof(TAggregateRoot).GetProperty(resourcePropertyInfo.Name, BindingFlags.Instance | BindingFlags.Public);

            if (aggregateRootPropertyInfo is null)
            {
                continue;
            }

            object? propertyValue = aggregateRootPropertyInfo.GetValue(aggregateRoot);

            if (propertyValue is null)
            {
                continue;
            }

            if (aggregateRootPropertyInfo.PropertyType.IsValueObject())
            {
                PropertyInfo? valueObjectValuePropertyInfo = aggregateRootPropertyInfo.PropertyType.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);

                if (valueObjectValuePropertyInfo is null)
                {
                    continue;
                }
                
                object? rawValue = valueObjectValuePropertyInfo.GetValue(propertyValue);
                
                resourcePropertyInfo.SetValue(resource, rawValue);

                continue;
            }

            if (aggregateRootPropertyInfo.PropertyType.IsResource())
            {
                // TODO
                
                continue;
            }

            if (aggregateRootPropertyInfo.PropertyType.IsEntityId())
            {
                Type relatedEntityIdType = aggregateRootPropertyInfo.PropertyType;
                Type relatedAggregateRootType = relatedEntityIdType.GetGenericArguments().Single();
                
                Uri relationUri = _routeGenerator.Item(relatedAggregateRootType, httpContext, Guid.Parse(propertyValue?.ToString() ?? string.Empty));
                Link relationLink = Link.RelationLink(relationUri);

                resource.Links.Add(resourcePropertyInfo.Name.Replace("Id", string.Empty).ToCamelCase(), relationLink);
            }

            resourcePropertyInfo.SetValue(resource, propertyValue);
        }

        return resource;
    }
}