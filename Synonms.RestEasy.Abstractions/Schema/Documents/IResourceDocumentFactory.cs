using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Schema.Documents;

public interface IResourceDocumentFactory<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    ResourceDocument<TAggregateRoot, TResource> Create(TResource resource, Uri documentUri, Func<TResource, string> resourceRelativePathFunc, Func<TResource, IReadOnlyDictionary<string, Link>>? resourceLinksFunc);
}