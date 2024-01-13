using Microsoft.AspNetCore.Authorization;

namespace Synonms.RestEasy.WebApi.Auth;

public interface IPolicyRegistrar
{
    void Register(AuthorizationOptions authorisationOptions);
}