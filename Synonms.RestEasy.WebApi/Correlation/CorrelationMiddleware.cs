using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Synonms.RestEasy.WebApi.Http;

namespace Synonms.RestEasy.WebApi.Correlation;

public class CorrelationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        if (httpContext.Items.ContainsKey(HttpContextItemKeys.RequestId) is false)
        {
            Guid requestId = Guid.NewGuid();
            
            httpContext.Items[HttpContextItemKeys.RequestId] = requestId;

            httpContext.Response.Headers[HttpHeaders.RequestId] = new StringValues(requestId.ToString());
        }
        
        if (httpContext.Items.ContainsKey(HttpContextItemKeys.CorrelationId) is false)
        {
            Guid correlationId = Guid.NewGuid();
            
            if (httpContext.Request.Headers.TryGetValue(HttpHeaders.CorrelationId, out StringValues correlationHeader))
            {
                if (Guid.TryParse(correlationHeader, out Guid incomingCorrelationId))
                {
                    correlationId = incomingCorrelationId;
                }
            }

            httpContext.Items[HttpContextItemKeys.CorrelationId] = correlationId;

            httpContext.Response.Headers[HttpHeaders.CorrelationId] = new StringValues(correlationId.ToString());
        }

        await next(httpContext);    
    }
}