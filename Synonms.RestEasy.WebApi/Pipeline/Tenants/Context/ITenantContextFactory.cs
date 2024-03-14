namespace Synonms.RestEasy.WebApi.Pipeline.Tenants.Context;

public interface ITenantContextFactory<TTenant>
    where TTenant : RestEasyTenant
{
    Task<TenantContext<TTenant>> CreateAsync(Guid? selectedTenantId, CancellationToken cancellationToken);
}