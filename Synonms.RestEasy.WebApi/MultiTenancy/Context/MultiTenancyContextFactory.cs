using Synonms.Functional;
using Synonms.RestEasy.WebApi.MultiTenancy.Faults;
using Synonms.RestEasy.WebApi.MultiTenancy.Persistence;

namespace Synonms.RestEasy.WebApi.MultiTenancy.Context;

public class MultiTenancyContextFactory<TTenant> : IMultiTenancyContextFactory<TTenant>
    where TTenant : Tenant
{
    private readonly ITenantRepository<TTenant> _repository;

    public MultiTenancyContextFactory(ITenantRepository<TTenant> repository)
    {
        _repository = repository;
    }
        
    public async Task<Result<MultiTenancyContext<TTenant>>> CreateAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        Maybe<TTenant> maybeTenant = await _repository.FindAsync(tenantId, cancellationToken);

        return maybeTenant.Match(
            tenant =>
            {
                MultiTenancyContext<TTenant> multiTenancyContext = MultiTenancyContext<TTenant>.Create(tenant);
                return Result<MultiTenancyContext<TTenant>>.Success(multiTenancyContext);
            },
            () =>
            {
                TenantResolutionFault fault = new ("Unable to find Tenant with id '{0}'.", new FaultSource(nameof(tenantId), tenantId.ToString()), tenantId);
                return Result<MultiTenancyContext<TTenant>>.Failure(fault);
            });
    }
}