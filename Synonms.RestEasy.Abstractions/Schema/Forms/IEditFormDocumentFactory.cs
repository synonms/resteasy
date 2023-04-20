using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Schema.Forms;

public interface IEditFormDocumentFactory<TAggregateRoot, in TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource, new()
{
    FormDocument Create(Uri documentUri, Uri targetUri, TResource resource);
}