using Microsoft.AspNetCore.Http;

namespace Synonms.RestEasy.WebApi.Versioning.Resolution;

public class VersionResolver : IVersionResolver
{
    private readonly IEnumerable<IVersionResolutionStrategy> _resolutionStrategies;

    public VersionResolver(IEnumerable<IVersionResolutionStrategy> resolutionStrategies)
    {
        _resolutionStrategies = resolutionStrategies;
    }
    
    public int Resolve(HttpRequest httpRequest)
    {
        foreach (IVersionResolutionStrategy resolutionStrategy in _resolutionStrategies)
        {
            if (resolutionStrategy.TryResolve(httpRequest, out int version))
            {
                return version;
            }
        }

        return VersioningConfiguration.DefaultVersion;
    }
}