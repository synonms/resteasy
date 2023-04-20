using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Schema.Forms;

public interface ICreateFormDocumentFactory<TAggregateRoot, in TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource, new()
{
    FormDocument Create(Uri documentUri, Uri targetUri, TResource? resource = null);
}