namespace Synonms.RestEasy.WebApi.Pipeline.Products.Context;

public interface IProductContextAccessor<TProduct>
    where TProduct : RestEasyProduct
{
    ProductContext<TProduct>? ProductContext { get; set; } 
}