using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.WebApi.Domain;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Schema.Forms;

public class CreateFormDocumentFactory<TAggregateRoot, TResource> : ICreateFormDocumentFactory<TAggregateRoot, TResource> 
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource, new()
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
        Link selfLink = Link.SelfLink(documentUri);
        
        return new FormDocument(selfLink, form);
    }
}