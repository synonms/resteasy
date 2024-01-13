using Microsoft.AspNetCore.Http;
using Synonms.Functional;
using Synonms.RestEasy.WebApi.Http;

namespace Synonms.RestEasy.WebApi.MultiTenancy;

public static class MultiTenancyHttpContextExtensions
{
    public static Maybe<TTenant> GetTenant<TTenant>(this HttpContext httpContext) where TTenant : Tenant
    {
        if (httpContext.Items.ContainsKey(HttpContextItemKeys.Tenant) is false)
        {
            return Maybe<TTenant>.None;
        }

        if (httpContext.Items[HttpContextItemKeys.Tenant] is TTenant tenant)
        {
            return tenant;
        }

        return Maybe<TTenant>.None;
    }
}