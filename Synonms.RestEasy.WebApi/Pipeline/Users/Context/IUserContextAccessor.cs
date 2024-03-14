namespace Synonms.RestEasy.WebApi.Pipeline.Users.Context;

public interface IUserContextAccessor<TUser>
    where TUser : RestEasyUser
{
    UserContext<TUser>? UserContext { get; set; } 
}