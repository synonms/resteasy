namespace Synonms.RestEasy.WebApi.Pipeline.Users.Context;

public interface IUserContextFactory<TUser>
    where TUser : RestEasyUser
{
    Task<UserContext<TUser>> CreateAsync(Guid? authenticatedUserId, CancellationToken cancellationToken);
}