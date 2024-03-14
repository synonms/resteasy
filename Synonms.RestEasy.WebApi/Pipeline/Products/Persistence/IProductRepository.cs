using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Pipeline.Products.Persistence;

public interface IProductRepository<TProduct>
    where TProduct : RestEasyProduct
{
    Task<IEnumerable<TProduct>> FindAvailableProductsAsync(CancellationToken cancellationToken);
    
    Task<Maybe<TProduct>> FindSelectedProductAsync(Guid id, CancellationToken cancellationToken);
}