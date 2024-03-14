using Synonms.Functional;
using Synonms.Functional.Extensions;

namespace Synonms.RestEasy.WebApi.Pipeline.Users.Resolution;

public class UserIdResolver : IUserIdResolver
{
    private readonly IEnumerable<IUserIdResolutionStrategy> _resolutionStrategies;

    public UserIdResolver(IEnumerable<IUserIdResolutionStrategy> resolutionStrategies)
    {
        _resolutionStrategies = resolutionStrategies;
    }
        
    public Task<Maybe<Guid>> ResolveAsync() =>
        Task.FromResult(_resolutionStrategies.Coalesce(strategy => strategy.Resolve(), Maybe<Guid>.None));
}