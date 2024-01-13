using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Synonms.Functional;
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
        if (httpContext.Request.Method == HttpMethods.Get)
        {
            string[] requestPathTokens = httpContext.Request.Path.ToString().Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (requestPathTokens.LastOrDefault()?.Equals("health", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                await next(httpContext);
                return;
            }
        }

        if (_multiTenancyContextAccessor.MultiTenancyContext is null)
        {
            Maybe<Guid> maybeTenantId = await _tenantIdResolver.ResolveAsync();
               
            Maybe<Fault> maybeFault = await maybeTenantId.Match(
                async tenantId =>
                {
                    Result<MultiTenancyContext<TTenant>> maybeMultiTenancyContext = await _multiTenancyContextFactory.CreateAsync(tenantId, CancellationToken.None);

                    return maybeMultiTenancyContext.Bind(
                        multiTenancyContext =>
                        {
                            _multiTenancyContextAccessor.MultiTenancyContext = multiTenancyContext;
                            return Maybe<Fault>.None;
                        });
                },
                () => Maybe<Fault>.SomeAsync(new TenantResolutionFault("Unable to resolve tenant code.")));

            if (maybeFault.IsSome)
            {
                maybeFault.IfSome(fault => _logger.LogError("MultiTenancy fault: {0}", fault));
                    
                httpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return;
            }
        }

        if (_multiTenancyContextAccessor.MultiTenancyContext is not null)
        {
            httpContext.Items[HttpContextItemKeys.Tenant] = _multiTenancyContextAccessor.MultiTenancyContext.Tenant;
            
            httpContext.Response.Headers[HttpHeaders.TenantId] = _multiTenancyContextAccessor.MultiTenancyContext.Tenant.Id.ToString();     
        }

        await next(httpContext);
    }
}