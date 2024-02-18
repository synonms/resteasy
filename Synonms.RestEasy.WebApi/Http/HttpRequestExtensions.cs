using Microsoft.AspNetCore.Http;

namespace Synonms.RestEasy.WebApi.Http;

public static class HttpRequestExtensions
{
    public static bool IsHealthCheck(this HttpRequest httpRequest)
    {
        if (httpRequest.Method == HttpMethods.Get)
        {
            string[] requestPathTokens = httpRequest.Path.ToString()
                .Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Where(token => token.Equals("/", StringComparison.OrdinalIgnoreCase) is false)
                .ToArray();
            
            if (requestPathTokens.LastOrDefault()?.Equals("health", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                return true;
            }
        }

        return false;
    }
}