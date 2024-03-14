using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Pipeline.Users.Resolution;

public interface IUserIdResolver
{
    Task<Maybe<Guid>> ResolveAsync();
}