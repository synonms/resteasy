using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Server;
using Synonms.RestEasy.Extensions;

namespace Synonms.RestEasy.Schema.Documents;

public class EditFormDocumentFactory<TAggregateRoot, TResource> : IEditFormDocumentFactory<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : ServerResource<TAggregateRoot>, new()
{
    private readonly ILookupOptionsProvider _lookupOptionsProvider;

    public EditFormDocumentFactory(ILookupOptionsProvider lookupOptionsProvider)
    {
        _lookupOptionsProvider = lookupOptionsProvider;
    }

    public FormDocument Create(Uri documentUri, Uri targetUri, TResource resource)
    {
        Form form = resource.GenerateEditForm<TAggregateRoot, TResource>(targetUri, _lookupOptionsProvider);
        Link selfLink = Link.EditFormLink(documentUri);
        
        return new FormDocument(selfLink, form);
    }
}