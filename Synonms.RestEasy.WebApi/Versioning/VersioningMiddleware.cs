using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Synonms.RestEasy.WebApi.Http;
using Synonms.RestEasy.WebApi.Versioning.Resolution;

namespace Synonms.RestEasy.WebApi.Versioning;

public class VersioningMiddleware : IMiddleware
{
    private readonly ILogger<VersioningMiddleware> _logger;
    private readonly IVersionResolver _versionResolver;

    public VersioningMiddleware(ILogger<VersioningMiddleware> logger, IVersionResolver versionResolver)
    {
        _logger = logger;
        _versionResolver = versionResolver;
    }
    
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        _logger.LogDebug("Executing Versioning Middleware...");

        if (httpContext.Items.ContainsKey(HttpContextItemKeys.ApiVersion))
        {
            _logger.LogDebug("API Version already present - skipping.");
        }
        else
        {
            int version = _versionResolver.Resolve(httpContext.Request);

            _logger.LogDebug("Resolved API Version {version}.", version);

            httpContext.Items[HttpContextItemKeys.ApiVersion] = version;

            httpContext.Response.Headers[HttpHeaders.ApiVersion] = version.ToString();
        }
        
        _logger.LogDebug("Versioning middleware complete.");
        await next(httpContext);    
    }
}