using Synonms.Functional;
using Synonms.Functional.Extensions;

namespace Synonms.RestEasy.WebApi.Pipeline.Products.Resolution;

public class ProductIdResolver : IProductIdResolver
{
    private readonly IEnumerable<IProductIdResolutionStrategy> _resolutionStrategies;

    public ProductIdResolver(IEnumerable<IProductIdResolutionStrategy> resolutionStrategies)
    {
        _resolutionStrategies = resolutionStrategies;
    }
        
    public Task<Maybe<Guid>> ResolveAsync() =>
        Task.FromResult(_resolutionStrategies.Coalesce(strategy => strategy.Resolve(), Maybe<Guid>.None));
}