using Synonms.RestEasy.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Synonms.RestEasy.WebApi.Http;

namespace Synonms.RestEasy.WebApi.Versioning.Resolution;

public class ApiVersionHeaderVersionResolutionStrategy : IVersionResolutionStrategy
{
    public bool TryResolve(HttpRequest httpRequest, out int version)
    {
        if (httpRequest.Headers.TryGetValue(HttpHeaders.ApiVersion, out StringValues apiVersionHeaderValues))
        {
            foreach (string? apiVersionHeaderValue in apiVersionHeaderValues)
            {
                if (apiVersionHeaderValue is not null && apiVersionHeaderValue.TryParsePositiveInt(out version))
                {
                    return true;
                }
            }
        }

        version = default;
        return false;
    }
}