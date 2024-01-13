using Synonms.RestEasy.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Synonms.RestEasy.WebApi.Http;

namespace Synonms.RestEasy.WebApi.Versioning.Resolution;

public class AcceptHeaderVersionResolutionStrategy : IVersionResolutionStrategy
{
    public bool TryResolve(HttpRequest httpRequest, out int version)
    {
        if (httpRequest.Headers.TryGetValue(HeaderNames.Accept, out StringValues acceptValues))
        {
            foreach (string? headerValue in acceptValues)
            {
                if (string.IsNullOrWhiteSpace(headerValue))
                {
                    continue;
                }

                if (headerValue.Contains(HttpHeaderKeys.ApiVersion, StringComparison.OrdinalIgnoreCase) is false)
                {
                    continue;
                }

                string[] acceptTokens = headerValue.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                foreach (string acceptToken in acceptTokens)
                {
                    string[] keyValueTokens = acceptToken.Split('=', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                    if (keyValueTokens.Length != 2)
                    {
                        continue;
                    }
                
                    if (keyValueTokens[0].Equals(HttpHeaderKeys.ApiVersion, StringComparison.OrdinalIgnoreCase) is false)
                    {
                        continue;
                    }

                    if (keyValueTokens[1].TryParsePositiveInt(out version))
                    {
                        return true;
                    }
                }
            }
        }
        
        version = default;
        return false;
    }
}