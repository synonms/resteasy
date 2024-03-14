using Synonms.Functional.Extensions;
using Synonms.RestEasy.WebApi.Pipeline.Products.Persistence;

namespace Synonms.RestEasy.WebApi.Pipeline.Products.Context;

public class ProductContextFactory<TProduct> : IProductContextFactory<TProduct>
    where TProduct : RestEasyProduct
{
    private readonly IProductRepository<TProduct> _repository;

    public ProductContextFactory(IProductRepository<TProduct> repository)
    {
        _repository = repository;
    }
        
    public async Task<ProductContext<TProduct>> CreateAsync(Guid? selectedProductId, CancellationToken cancellationToken)
    {
        TProduct? selectedProduct = null;
        
        if (selectedProductId is not null)
        {
            await _repository.FindSelectedProductAsync(selectedProductId.Value, cancellationToken)
                .IfSomeAsync(product => selectedProduct = product);
        }

        IEnumerable<TProduct> availableProducts = await _repository.FindAvailableProductsAsync(cancellationToken);

        return ProductContext<TProduct>.Create(availableProducts, selectedProduct);
    }
}