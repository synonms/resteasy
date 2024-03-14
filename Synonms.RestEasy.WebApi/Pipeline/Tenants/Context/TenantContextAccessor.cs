namespace Synonms.RestEasy.WebApi.Pipeline.Tenants.Context;

public class TenantContextAccessor<TTenant> : ITenantContextAccessor<TTenant>
    where TTenant : RestEasyTenant
{
    public TenantContext<TTenant>? TenantContext { get; set; }
}