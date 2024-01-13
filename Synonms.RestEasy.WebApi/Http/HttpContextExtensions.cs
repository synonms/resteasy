using Microsoft.AspNetCore.Http;
using Synonms.RestEasy.WebApi.Versioning;

namespace Synonms.RestEasy.WebApi.Http;

public static class HttpContextExtensions
{
    public static int GetApiVersion(this HttpContext httpContext)
    {
        if (httpContext.Items.ContainsKey(HttpContextItemKeys.ApiVersion) is false)
        {
            return 1;
        }
        
        int version = httpContext.Items[HttpContextItemKeys.ApiVersion] as int? ?? 1;

        return version <= 0 ? VersioningConfiguration.DefaultVersion : version;
    }
    
    public static Guid? GetCorrelationId(this HttpContext httpContext)
    {
        if (httpContext.Items.ContainsKey(HttpContextItemKeys.CorrelationId) is false)
        {
            return null;
        }
        
        Guid? correlationId = httpContext.Items[HttpContextItemKeys.CorrelationId] as Guid?;

        if (correlationId is null)
        {
            return null;
        }

        return correlationId.Equals(Guid.Empty) ? null : correlationId;
    }
    
    public static Guid? GetRequestId(this HttpContext httpContext)
    {
        if (httpContext.Items.ContainsKey(HttpContextItemKeys.RequestId) is false)
        {
            return null;
        }
        
        Guid? requestId = httpContext.Items[HttpContextItemKeys.RequestId] as Guid?;

        if (requestId is null)
        {
            return null;
        }

        return requestId.Equals(Guid.Empty) ? null : requestId;
    }
}