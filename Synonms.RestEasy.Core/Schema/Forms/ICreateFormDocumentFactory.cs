using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.Core.Schema.Forms;

public interface ICreateFormDocumentFactory<TAggregateRoot, in TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource, new()
{
    FormDocument Create(Uri documentUri, Uri targetUri, TResource? resource = null);
}