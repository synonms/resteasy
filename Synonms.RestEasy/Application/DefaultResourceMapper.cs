using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Http;
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
    where TResource : Resource<TAggregateRoot>, new() 
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
            Id = aggregateRoot.Id,
            SelfLink = selfLink
        };

        resource.Links.Add(IanaLinkRelations.Forms.Edit, editFormLink);
        resource.Links.Add(IanaHttpMethods.Delete.ToLowerInvariant(), deleteSelfLink);
        
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
            
            if (aggregateRootPropertyInfo.PropertyType.IsAggregateMember())
            {
                Type aggregateMemberType = aggregateRootPropertyInfo.PropertyType;
                Type childResourceType = resourcePropertyInfo.PropertyType;

                object? childResource = MapAggregateMember(aggregateMemberType, childResourceType, httpContext, propertyValue);

                resourcePropertyInfo.SetValue(resource, childResource);
                
                continue;
            }

            if (aggregateRootPropertyInfo.PropertyType.IsArrayOrEnumerable())
            {
                Type? aggregateMemberType = aggregateRootPropertyInfo.PropertyType.GetArrayOrEnumerableElementType();

                if (aggregateMemberType is null || aggregateMemberType.IsAggregateMember() is false)
                {
                    continue;
                }

                Type? childResourceType = resourcePropertyInfo.PropertyType.GetArrayOrEnumerableElementType();

                if (childResourceType is null || childResourceType.IsChildResource() is false)
                {
                    continue;
                }

                Type childResourceCollectionType = typeof(List<>).MakeGenericType(childResourceType);

                IList childResources = (IList)Activator.CreateInstance(childResourceCollectionType);
                
                if (propertyValue is IEnumerable enumerablePropertyValue)
                {
                    foreach (object item in enumerablePropertyValue)
                    {
                        var childResource = MapAggregateMember(aggregateMemberType, childResourceType, httpContext, item);
//                        var childResource = Convert.ChangeType(mappedObject, childResourceType);
                        
                        if (childResource is not null)
                        {
                            childResources?.Add(childResource);
                        }
                    }
                }

                resourcePropertyInfo.SetValue(resource, childResources);

                continue;
            }
            
            if (aggregateRootPropertyInfo.PropertyType.IsEntityId())
            {
                Type relatedEntityIdType = aggregateRootPropertyInfo.PropertyType;
                Type relatedEntityType = relatedEntityIdType.GetGenericArguments().Single();
                
                Uri relationUri = _routeGenerator.Item(relatedEntityType, httpContext, Guid.Parse(propertyValue?.ToString() ?? string.Empty));
                Link relationLink = Link.RelationLink(relationUri);

                resource.Links.Add(resourcePropertyInfo.Name.Replace("Id", string.Empty).ToCamelCase(), relationLink);
            }

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