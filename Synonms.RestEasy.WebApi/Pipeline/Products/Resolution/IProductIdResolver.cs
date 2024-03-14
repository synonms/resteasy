using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Pipeline.Products.Resolution;

public interface IProductIdResolver
{
    Task<Maybe<Guid>> ResolveAsync();
}