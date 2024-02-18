using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Synonms.Functional;
using Synonms.RestEasy.WebApi.Http;

namespace Synonms.RestEasy.WebApi.Auth;

public class PermissionsMiddleware : IMiddleware
{
    private readonly ILogger<PermissionsMiddleware> _logger;
    private readonly IPermissionRepository _permissionRepository;

    public PermissionsMiddleware(ILogger<PermissionsMiddleware> logger, IPermissionRepository permissionRepository)
    {
        _logger = logger;
        _permissionRepository = permissionRepository;
    }
    
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        if (httpContext.User.Identity is not { IsAuthenticated: true })
        {
            await next(httpContext);
            return;
        }

        if (httpContext.Request.IsHealthCheck())
        {
            await next(httpContext);
            return;
        }

        CancellationToken cancellationToken = httpContext.RequestAborted;

        Maybe<Fault> outcome = (await _permissionRepository.ReadPermissionsAsync(httpContext.User, cancellationToken))
            .Match(claimsIdentity =>
                {
                    httpContext.User.AddIdentity(claimsIdentity);
                    return Maybe<Fault>.None;
                },
                fault =>
                {
                    _logger.LogError("Permissions fault: {fault}", fault);
                    return fault;
                });
        
        if (outcome.IsSome)
        {
            httpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            return;
        }
        
        await next(httpContext);
    }
}