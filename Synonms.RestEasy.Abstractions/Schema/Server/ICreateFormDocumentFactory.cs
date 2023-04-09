using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Schema.Server;

public interface ICreateFormDocumentFactory<TAggregateRoot, in TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : ServerResource<TAggregateRoot>, new()
{
    FormDocument Create(Uri documentUri, Uri targetUri, TResource? resource = null);
}