using System.Security.Claims;

namespace Synonms.RestEasy.Blazor.Auth;

public interface IPermissionProvider
{
    Task<ClaimsIdentity?> GetAsync(Guid userId, CancellationToken cancellationToken);
}