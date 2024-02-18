using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Runtime;
using Synonms.RestEasy.Core.Schema.Forms;

namespace Synonms.RestEasy.Core.Schema.Resources;

public static class ResourceExtensions
{
    public static Form GenerateCreateForm<TAggregateRoot, TResource>(this TResource resource, Uri targetUri, ILookupOptionsProvider lookupOptionsProvider)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>
        where TResource : Resource, new()
    {
        Link targetLink = Link.CreateFormTargetLink(targetUri);

        List<FormField> formFields = resource.GetFormFields(lookupOptionsProvider).ToList();

        return new Form(targetLink, formFields);
    }
    
    public static Form GenerateEditForm<TAggregateRoot, TResource>(this TResource resource, Uri targetUri, ILookupOptionsProvider lookupOptionsProvider)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>
        where TResource : Resource, new()
    {
        Link targetLink = Link.EditFormTargetLink(targetUri);

        List<FormField> formFields = resource.GetFormFields(lookupOptionsProvider).ToList();

        return new Form(targetLink, formFields);
    }
}