using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Synonms.RestEasy.Testing.Auth;

namespace Synonms.RestEasy.Testing.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ReplaceAuthentication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAuthentication(authenticationOptions =>
            {
                authenticationOptions.DefaultAuthenticateScheme = TestAuthenticationHandler.AuthenticationScheme;
                authenticationOptions.DefaultChallengeScheme = TestAuthenticationHandler.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.AuthenticationScheme, _ => { });

        return serviceCollection;
    }
}