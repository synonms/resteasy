using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.MultiTenancy.Resolution;

public interface ITenantIdResolver
{
    Task<Maybe<Guid>> ResolveAsync();
}