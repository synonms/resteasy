using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.SharedKernel.Collections;

namespace Synonms.RestEasy.Abstractions.Schema.Documents;

public interface IResourceCollectionDocumentFactory<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    ResourceCollectionDocument<TAggregateRoot, TResource> Create(PaginatedList<TResource> resourceCollection, Func<int, Uri> documentUriFunc, Func<TResource, string> resourceRelativePathFunc, Func<TResource, IReadOnlyDictionary<string, Link>>? resourceLinksFunc);
}