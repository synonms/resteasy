using System.Security.Claims;
using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Auth;

public interface IPermissionRepository
{
    Task<Result<ClaimsIdentity>> ReadPermissionsAsync(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken);
}