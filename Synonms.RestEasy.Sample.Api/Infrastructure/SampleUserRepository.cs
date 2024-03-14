using Synonms.Functional;
using Synonms.RestEasy.WebApi.Pipeline.Users.Persistence;

namespace Synonms.RestEasy.Sample.Api.Infrastructure;

public class SampleUserRepository : IUserRepository<SampleUser>
{
    public Task<Maybe<SampleUser>> FindAuthenticatedUserAsync(CancellationToken cancellationToken) => 
        Task.FromResult(Maybe<SampleUser>.Some(new SampleUser()));
}