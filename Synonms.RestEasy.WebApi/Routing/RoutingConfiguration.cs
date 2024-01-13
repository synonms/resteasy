using Microsoft.AspNetCore.Routing;

namespace Synonms.RestEasy.WebApi.Routing;

public static class RoutingConfiguration
{
    public static readonly LinkOptions DefaultLinkOptions = new()
    {
        LowercaseUrls = true
    };
}