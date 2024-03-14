using Synonms.Functional.Extensions;
using Synonms.RestEasy.WebApi.Pipeline.Tenants.Persistence;

namespace Synonms.RestEasy.WebApi.Pipeline.Tenants.Context;

public class TenantContextFactory<TTenant> : ITenantContextFactory<TTenant>
    where TTenant : RestEasyTenant
{
    private readonly ITenantRepository<TTenant> _repository;

    public TenantContextFactory(ITenantRepository<TTenant> repository)
    {
        _repository = repository;
    }
        
    public async Task<TenantContext<TTenant>> CreateAsync(Guid? selectedTenantId, CancellationToken cancellationToken)
    {
        TTenant? selectedTenant = null;
        
        if (selectedTenantId is not null)
        {
            await _repository.FindSelectedTenantAsync(selectedTenantId.Value, cancellationToken)
                .IfSomeAsync(tenant => selectedTenant = tenant);
        }

        IEnumerable<TTenant> availableTenants = await _repository.FindAvailableTenantsAsync(cancellationToken);

        return TenantContext<TTenant>.Create(availableTenants, selectedTenant);
    }
}