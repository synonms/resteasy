using Synonms.Functional;

namespace Synonms.RestEasy.Sample.Api.Infrastructure;

public static class FunctionalHelper
{
    public static T FromResult<T>(Result<T> result) =>
        result.Match(
            entity => entity,
            fault => throw new Exception($"Failed to create {typeof(T).Name}: " + fault));
}