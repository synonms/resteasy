namespace Synonms.RestEasy.WebApi.Pipeline.Products.Context;

public interface IProductContextFactory<TProduct>
    where TProduct : RestEasyProduct
{
    Task<ProductContext<TProduct>> CreateAsync(Guid? selectedProductId, CancellationToken cancellationToken);
}