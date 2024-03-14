using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Pipeline.Tenants.Resolution;

public interface ITenantIdResolutionStrategy
{
    Maybe<Guid> Resolve();
}