using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Extensions;

namespace Synonms.RestEasy.Schema.Documents;

public class CreateFormDocumentFactory<TAggregateRoot, TResource> : ICreateFormDocumentFactory<TAggregateRoot, TResource> 
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>, new()
{
    private readonly ILookupOptionsProvider _lookupOptionsProvider;

    public CreateFormDocumentFactory(ILookupOptionsProvider lookupOptionsProvider)
    {
        _lookupOptionsProvider = lookupOptionsProvider;
    }
    
    public FormDocument Create(Uri documentUri, Uri targetUri, TResource? resource = null)
    {
        resource ??= new TResource();
        Form form = resource.GenerateCreateForm<TAggregateRoot, TResource>(targetUri, _lookupOptionsProvider);
        Link selfLink = Link.CreateFormLink(documentUri);
        
        return new FormDocument(selfLink, form);
    }
}