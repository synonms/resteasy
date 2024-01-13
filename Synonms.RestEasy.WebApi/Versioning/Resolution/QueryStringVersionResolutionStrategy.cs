using Synonms.RestEasy.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Synonms.RestEasy.WebApi.Http;

namespace Synonms.RestEasy.WebApi.Versioning.Resolution;

public class QueryStringVersionResolutionStrategy : IVersionResolutionStrategy
{
    public bool TryResolve(HttpRequest httpRequest, out int version)
    {
        if (httpRequest.Query.TryGetValue(HttpQueryStringKeys.ApiVersion, out StringValues apiVersionValues))
        {
            foreach (string? queryStringValue in apiVersionValues)
            {
                if (queryStringValue is not null && queryStringValue.TryParsePositiveInt(out version))
                {
                    return true;
                }
            }
        }
        
        version = default;
        return false;
    }
}