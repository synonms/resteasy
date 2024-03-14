using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Pipeline.Users.Persistence;

public interface IUserRepository<TUser>
    where TUser : RestEasyUser
{
    Task<Maybe<TUser>> FindAuthenticatedUserAsync(CancellationToken cancellationToken);
}