using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.MultiTenancy.Context;

public interface IMultiTenancyContextFactory<TTenant>
    where TTenant : Tenant
{
    Task<Result<MultiTenancyContext<TTenant>>> CreateAsync(Guid tenantId, CancellationToken cancellationToken);
}