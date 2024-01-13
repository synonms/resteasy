using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.MultiTenancy.Persistence;

public interface ITenantRepository<TTenant>
    where TTenant : Tenant
{
    Task<IEnumerable<TTenant>> GetAllAsync(CancellationToken cancellationToken);

    Task<Maybe<TTenant>> FindAsync(Guid id, CancellationToken cancellationToken);
}