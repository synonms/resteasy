namespace Synonms.RestEasy.WebApi.Pipeline.Tenants.Context;

public interface ITenantContextAccessor<TTenant>
    where TTenant : RestEasyTenant
{
    TenantContext<TTenant>? TenantContext { get; set; } 
}