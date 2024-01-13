using Microsoft.AspNetCore.Http;

namespace Synonms.RestEasy.WebApi.Versioning.Resolution;

public interface IVersionResolutionStrategy
{
    bool TryResolve(HttpRequest httpRequest, out int version);
}