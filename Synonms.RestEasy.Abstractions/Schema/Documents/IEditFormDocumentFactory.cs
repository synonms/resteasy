using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Schema.Documents;

public interface IEditFormDocumentFactory<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    FormDocument Create(Uri documentUri, Uri targetUri, Resource<TAggregateRoot> resource);
}