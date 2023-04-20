using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Extensions;
using Synonms.RestEasy.SharedKernel.Extensions;

namespace Synonms.RestEasy.Application;

public class DefaultResourceMapper<TAggregateRoot, TResource> : IResourceMapper<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource, new() 
{
    private readonly IChildResourceMapperFactory _childResourceMapperFactory;
    private readonly IRouteGenerator _routeGenerator;
    
    public DefaultResourceMapper(IChildResourceMapperFactory childResourceMapperFactory, IRouteGenerator routeGenerator)
    {
        _childResourceMapperFactory = childResourceMapperFactory;
        _routeGenerator = routeGenerator;
    }
    
    public TResource Map(HttpContext httpContext, TAggregateRoot aggregateRoot)
    {
        Uri selfUri = _routeGenerator.Item(httpContext, aggregateRoot.Id);
        Link selfLink = Link.SelfLink(selfUri);
        Uri editFormUri = _routeGenerator.EditForm(httpContext, aggregateRoot.Id);
        Link editFormLink = Link.EditFormLink(editFormUri);
        Link deleteSelfLink = Link.DeleteSelfLink(selfUri);

        TResource resource = new()
        {
            Id = aggregateRoot.Id.Value,
            SelfLink = selfLink
        };

        resource.Links.Add(IanaLinkRelations.Forms.Edit, editFormLink);
        resource.Links.Add(IanaHttpMethods.Delete.ToLowerInvariant(), deleteSelfLink);
        
        foreach (PropertyInfo resourcePropertyInfo in typeof(TResource).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            // Ignore internal resource properties
            if (resourcePropertyInfo.Name.Equals("Id") || resourcePropertyInfo.Name.Equals("SelfLink") || resourcePropertyInfo.Name.Equals("Links"))
            {
                continue;
            }
            
            PropertyInfo? aggregateRootPropertyInfo = typeof(TAggregateRoot).GetProperty(resourcePropertyInfo.Name, BindingFlags.Instance | BindingFlags.Public);
            object? propertyValue = aggregateRootPropertyInfo?.GetValue(aggregateRoot);

            if (resourcePropertyInfo.PropertyType.IsArrayOrEnumerable())
            {
                Type? resourcePropertyEnumerableElementType = resourcePropertyInfo.PropertyType.GetArrayOrEnumerableElementType();

                if (resourcePropertyEnumerableElementType is null)
                {
                    // TODO: Warn
                    continue;
                }
                
                if (resourcePropertyEnumerableElementType.IsEntityId())
                {
                    // TResource.IEnumerable<EntityId<TAggregateRoot>> = A related resource collection where we present a link.
                    // We only need the Id from the Aggregate for this, not a related property value.
                    
                    Type childEntityType = resourcePropertyEnumerableElementType.GetGenericArguments().Single();
                    EntityId<TAggregateRoot> parentId = aggregateRoot.Id;
                    string parentIdPropertyName = typeof(TAggregateRoot).Name.ToCamelCase() + "Id";
                    QueryCollection query = new(
                        new Dictionary<string, StringValues>
                        {
                            [parentIdPropertyName] = new(parentId.Value.ToString())
                        });

                    Uri relationUri = _routeGenerator.Collection(childEntityType, httpContext, query);
                    Link relationLink = Link.RelationLink(relationUri);

                    resource.Links.Add(resourcePropertyInfo.Name.ToCamelCase(), relationLink);

                    continue;
                }
                
                if (aggregateRootPropertyInfo is null)
                {
                    // TODO: Warn
                    continue;
                }

                Type? aggregateRootPropertyEnumerableElementType = aggregateRootPropertyInfo.PropertyType.GetArrayOrEnumerableElementType();

                if (aggregateRootPropertyEnumerableElementType is null || propertyValue is null)
                {
                    // TODO: Warn
                    continue;
                }

                if (aggregateRootPropertyEnumerableElementType.IsAggregateMember())
                {
                    // TAggregateRoot.IEnumerable<TAggregateMember> = A member collection where we present a nested child resource array.
                    
                    if (resourcePropertyEnumerableElementType.IsChildResource() is false)
                    {
                        // TODO: Warn
                        continue;
                    }

                    Type childResourceCollectionType = typeof(List<>).MakeGenericType(resourcePropertyEnumerableElementType);

                    IList childResources = (IList)Activator.CreateInstance(childResourceCollectionType);
                
                    if (propertyValue is IEnumerable enumerablePropertyValue)
                    {
                        foreach (object item in enumerablePropertyValue)
                        {
                            var childResource = MapAggregateMember(aggregateRootPropertyEnumerableElementType, resourcePropertyEnumerableElementType, httpContext, item);
                        
                            if (childResource is not null)
                            {
                                childResources?.Add(childResource);
                            }
                        }
                    }

                    resourcePropertyInfo.SetValue(resource, childResources);
                    
                    continue;
                }

                // TODO: Array which is not EntityId<>[] (link) or AggregateMember[] (nested resource)
                continue;
            }

            if (aggregateRootPropertyInfo is null)
            {
                // TODO: Warn
                continue;
            }

            if (aggregateRootPropertyInfo.PropertyType.IsEntityId())
            {
                // TAggregateRoot.EntityId<TEntity> = A related resource where we present a link.

                Type relatedEntityIdType = aggregateRootPropertyInfo.PropertyType;
                Type relatedEntityType = relatedEntityIdType.GetGenericArguments().Single();
                
                Uri relationUri = _routeGenerator.Item(relatedEntityType, httpContext, Guid.Parse(propertyValue?.ToString() ?? Guid.Empty.ToString()));
                Link relationLink = Link.RelationLink(relationUri);

                resource.Links.Add(resourcePropertyInfo.Name.Replace("Id", string.Empty).ToCamelCase(), relationLink);
            }

            if (aggregateRootPropertyInfo.PropertyType.IsAggregateMember())
            {
                // TAggregateRoot.TAggregateMember = A member where we present a nested child resource.
                
                Type aggregateMemberType = aggregateRootPropertyInfo.PropertyType;
                Type childResourceType = resourcePropertyInfo.PropertyType;

                object? childResource = MapAggregateMember(aggregateMemberType, childResourceType, httpContext, propertyValue);

                resourcePropertyInfo.SetValue(resource, childResource);
                
                continue;
            }

            if (aggregateRootPropertyInfo.PropertyType.IsValueObject())
            {
                // TAggregateRoot.ValueObject - A DDD value object property which we cast to a regular resource property
                
                PropertyInfo? valueObjectValuePropertyInfo = aggregateRootPropertyInfo.PropertyType.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);

                if (valueObjectValuePropertyInfo is null)
                {
                    continue;
                }
                
                object? rawValue = propertyValue is null ? null : valueObjectValuePropertyInfo.GetValue(propertyValue);
                
                resourcePropertyInfo.SetValue(resource, rawValue);

                continue;
            }

            // If all else fails, assume a vanilla property (bool, string, int etc.)

            resourcePropertyInfo.SetValue(resource, propertyValue);
        }

        return resource;
    }

    private object? MapAggregateMember(Type aggregateMemberType, Type childResourceType, HttpContext httpContext, object? aggregateMemberValue)
    {
        if (aggregateMemberValue is null)
        {
            return null;
        }
        
        if (childResourceType.IsChildResource() is false)
        {
            return null;
        }

        IChildResourceMapper? childResourceMapper = _childResourceMapperFactory.Create(aggregateMemberType, childResourceType);

        return childResourceMapper?.Map(httpContext, aggregateMemberValue);
    }
}