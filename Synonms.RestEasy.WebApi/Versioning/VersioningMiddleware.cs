using Microsoft.AspNetCore.Http;
using Synonms.RestEasy.WebApi.Http;
using Synonms.RestEasy.WebApi.Versioning.Resolution;

namespace Synonms.RestEasy.WebApi.Versioning;

public class VersioningMiddleware : IMiddleware
{
    private readonly IVersionResolver _versionResolver;

    public VersioningMiddleware(IVersionResolver versionResolver)
    {
        _versionResolver = versionResolver;
    }
    
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        if (httpContext.Items.ContainsKey(HttpContextItemKeys.ApiVersion) is false)
        {
            int version = _versionResolver.Resolve(httpContext.Request);
        
            httpContext.Items[HttpContextItemKeys.ApiVersion] = version;

            httpContext.Response.Headers[HttpHeaders.ApiVersion] = version.ToString();
        }
        
        await next(httpContext);    
    }
}