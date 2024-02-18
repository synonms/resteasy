using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.WebApi.Http;
using Synonms.RestEasy.WebApi.MultiTenancy.Context;
using Synonms.RestEasy.WebApi.MultiTenancy.Faults;
using Synonms.RestEasy.WebApi.MultiTenancy.Resolution;

namespace Synonms.RestEasy.WebApi.MultiTenancy;

public class MultiTenancyMiddleware<TTenant> : IMiddleware
    where TTenant : Tenant
{
    private readonly ILogger _logger;
    private readonly ITenantIdResolver _tenantIdResolver;
    private readonly IMultiTenancyContextFactory<TTenant> _multiTenancyContextFactory;
    private readonly IMultiTenancyContextAccessor<TTenant> _multiTenancyContextAccessor;

    public MultiTenancyMiddleware(
        ILogger<MultiTenancyMiddleware<TTenant>> logger, 
        ITenantIdResolver tenantIdResolver, 
        IMultiTenancyContextFactory<TTenant> multiTenancyContextFactory, 
        IMultiTenancyContextAccessor<TTenant> multiTenancyContextAccessor)
    {
        _logger = logger;
        _tenantIdResolver = tenantIdResolver;
        _multiTenancyContextFactory = multiTenancyContextFactory;
        _multiTenancyContextAccessor = multiTenancyContextAccessor;
    }

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        if (httpContext.Request.IsHealthCheck())
        {
            await next(httpContext);
            return;
        }

        if (_multiTenancyContextAccessor.MultiTenancyContext is null)
        {
            (await _tenantIdResolver
                .ResolveAsync()
                .MatchAsync(
                    async tenantId => await _multiTenancyContextFactory.CreateAsync(tenantId, CancellationToken.None),
                    () => Result<MultiTenancyContext<TTenant>>.FailureAsync(new TenantResolutionFault("Unable to resolve tenant Id."))))
                .Match(
                    multiTenancyContext => _multiTenancyContextAccessor.MultiTenancyContext = multiTenancyContext,
                    fault => _logger.LogDebug("MultiTenancy Fault: {fault}", fault));
        }

        if (_multiTenancyContextAccessor.MultiTenancyContext is not null)
        {
            httpContext.Items[HttpContextItemKeys.Tenant] = _multiTenancyContextAccessor.MultiTenancyContext.Tenant;
            
            httpContext.Response.Headers[HttpHeaders.TenantId] = _multiTenancyContextAccessor.MultiTenancyContext.Tenant.Id.ToString();     
        }

        await next(httpContext);
    }
}