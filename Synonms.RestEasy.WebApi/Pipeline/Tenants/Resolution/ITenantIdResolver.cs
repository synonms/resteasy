using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Pipeline.Tenants.Resolution;

public interface ITenantIdResolver
{
    Task<Maybe<Guid>> ResolveAsync();
}