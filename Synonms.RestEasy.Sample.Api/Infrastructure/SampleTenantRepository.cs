using Synonms.Functional;
using Synonms.RestEasy.WebApi.Pipeline.Tenants.Persistence;

namespace Synonms.RestEasy.Sample.Api.Infrastructure;

public class SampleTenantRepository : ITenantRepository<SampleTenant>
{
    public Task<IEnumerable<SampleTenant>> FindAvailableTenantsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Enumerable.Empty<SampleTenant>());

    public Task<Maybe<SampleTenant>> FindSelectedTenantAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Maybe<SampleTenant>.None);
}