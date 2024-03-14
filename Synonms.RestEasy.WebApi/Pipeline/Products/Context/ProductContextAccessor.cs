namespace Synonms.RestEasy.WebApi.Pipeline.Products.Context;

public class ProductContextAccessor<TProduct> : IProductContextAccessor<TProduct>
    where TProduct : RestEasyProduct
{
    public ProductContext<TProduct>? ProductContext { get; set; }
}