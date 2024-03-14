using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Pipeline.Products.Resolution;

public interface IProductIdResolutionStrategy
{
    Maybe<Guid> Resolve();
}