using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Pipeline.Tenants.Persistence;

public interface ITenantRepository<TTenant>
    where TTenant : RestEasyTenant
{
    Task<IEnumerable<TTenant>> FindAvailableTenantsAsync(CancellationToken cancellationToken);
    
    Task<Maybe<TTenant>> FindSelectedTenantAsync(Guid id, CancellationToken cancellationToken);
}