using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.MultiTenancy.Resolution;

public interface ITenantIdResolutionStrategy
{
    Maybe<Guid> Resolve();
}