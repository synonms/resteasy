using Microsoft.AspNetCore.Routing;

namespace Synonms.RestEasy.Routing;

public static class RoutingConstants
{
    public static readonly LinkOptions DefaultLinkOptions = new ()
    {
        LowercaseUrls = true
    };
}