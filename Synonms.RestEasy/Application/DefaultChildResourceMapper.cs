using System.Reflection;
using Microsoft.AspNetCore.Http;
using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Extensions;

namespace Synonms.RestEasy.Application;

public class DefaultChildResourceMapper<TAggregateMember, TChildResource> : IChildResourceMapper<TAggregateMember, TChildResource>
    where TAggregateMember : AggregateMember<TAggregateMember>
    where TChildResource : ChildResource, new()
{
    public object? Map(HttpContext httpContext, object value)
    {
        if (value is TAggregateMember aggregateMember)
        {
            return Map(httpContext, aggregateMember);
        }
        
        return null;
    }

    public TChildResource? Map(HttpContext httpContext, TAggregateMember aggregateMember)
    {
        TChildResource childResource = new()
        {
            Id = aggregateMember.Id.Value,
        };

        foreach (PropertyInfo resourcePropertyInfo in typeof(TChildResource).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (resourcePropertyInfo.Name.Equals("Id"))
            {
                continue;
            }
            
            PropertyInfo? aggregateMemberPropertyInfo = typeof(TAggregateMember).GetProperty(resourcePropertyInfo.Name, BindingFlags.Instance | BindingFlags.Public);

            if (aggregateMemberPropertyInfo is null)
            {
                continue;
            }

            object? propertyValue = aggregateMemberPropertyInfo.GetValue(aggregateMember);

            if (propertyValue is null)
            {
                continue;
            }

            if (aggregateMemberPropertyInfo.PropertyType.IsValueObject())
            {
                PropertyInfo? valueObjectValuePropertyInfo = aggregateMemberPropertyInfo.PropertyType.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);

                if (valueObjectValuePropertyInfo is null)
                {
                    continue;
                }
                
                object? rawValue = valueObjectValuePropertyInfo.GetValue(propertyValue);
                
                resourcePropertyInfo.SetValue(childResource, rawValue);

                continue;
            }

            if (aggregateMemberPropertyInfo.PropertyType.IsChildResource())
            {
                // TODO: Support deep nesting?
                
                continue;
            }

            if (aggregateMemberPropertyInfo.PropertyType.IsEntityId())
            {
                // TODO: Support links on nested resources?
                
                continue;
            }

            resourcePropertyInfo.SetValue(childResource, propertyValue);
        }

        return childResource;
    }
}