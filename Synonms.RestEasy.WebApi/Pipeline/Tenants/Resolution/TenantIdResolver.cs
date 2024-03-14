using Synonms.Functional;
using Synonms.Functional.Extensions;

namespace Synonms.RestEasy.WebApi.Pipeline.Tenants.Resolution;

public class TenantIdResolver : ITenantIdResolver
{
    private readonly IEnumerable<ITenantIdResolutionStrategy> _resolutionStrategies;

    public TenantIdResolver(IEnumerable<ITenantIdResolutionStrategy> resolutionStrategies)
    {
        _resolutionStrategies = resolutionStrategies;
    }
        
    public Task<Maybe<Guid>> ResolveAsync() =>
        Task.FromResult(_resolutionStrategies.Coalesce(strategy => strategy.Resolve(), Maybe<Guid>.None));
}