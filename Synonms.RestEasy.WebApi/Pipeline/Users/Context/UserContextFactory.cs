using Synonms.RestEasy.WebApi.Pipeline.Users.Persistence;

namespace Synonms.RestEasy.WebApi.Pipeline.Users.Context;

public class UserContextFactory<TUser> : IUserContextFactory<TUser>
    where TUser : RestEasyUser
{
    private readonly IUserRepository<TUser> _repository;

    public UserContextFactory(IUserRepository<TUser> repository)
    {
        _repository = repository;
    }
        
    public async Task<UserContext<TUser>> CreateAsync(Guid? authenticatedUserId, CancellationToken cancellationToken) =>
        (await _repository.FindAuthenticatedUserAsync(cancellationToken))
            .Match(
                UserContext<TUser>.Create, 
                () => UserContext<TUser>.Create(null));
}