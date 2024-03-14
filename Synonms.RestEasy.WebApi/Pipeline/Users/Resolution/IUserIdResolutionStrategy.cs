using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Pipeline.Users.Resolution;

public interface IUserIdResolutionStrategy
{
    Maybe<Guid> Resolve();
}