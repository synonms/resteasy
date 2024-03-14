namespace Synonms.RestEasy.WebApi.Pipeline.Users.Context;

public class UserContext<TUser>
    where TUser : RestEasyUser
{
    private UserContext(TUser? authenticatedUser)
    {
        AuthenticatedUser = authenticatedUser;
    }
    
    public TUser? AuthenticatedUser { get; }

    public static UserContext<TUser> Create(TUser? authenticatedUSer) =>
        new (authenticatedUSer);
}