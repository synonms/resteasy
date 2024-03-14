using Synonms.Functional;
using Synonms.RestEasy.WebApi.Pipeline.Products.Persistence;

namespace Synonms.RestEasy.Sample.Api.Infrastructure;

public class SampleProductRepository : IProductRepository<SampleProduct>
{
    public Task<IEnumerable<SampleProduct>> FindAvailableProductsAsync(CancellationToken cancellationToken) => 
        Task.FromResult(Enumerable.Empty<SampleProduct>());

    public Task<Maybe<SampleProduct>> FindSelectedProductAsync(Guid id, CancellationToken cancellationToken) => 
        Task.FromResult(Maybe<SampleProduct>.None);
}