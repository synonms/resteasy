namespace Synonms.RestEasy.WebApi.Pipeline.Users.Context;

public class UserContextAccessor<TUser> : IUserContextAccessor<TUser>
    where TUser : RestEasyUser
{
    public UserContext<TUser>? UserContext { get; set; }
}