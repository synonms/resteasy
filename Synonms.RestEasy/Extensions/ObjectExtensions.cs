using System.Reflection;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema.Forms;
using Synonms.RestEasy.Serialisation.Ion.Extensions;

namespace Synonms.RestEasy.Extensions;

public static class ObjectExtensions
{
    private static readonly string[] FormsIgnorePropertyNames =
    {
        "SelfLink",
        "Links"
    };
    
    public static IEnumerable<FormField> GetFormFields(this object instance, ILookupOptionsProvider lookupOptionsProvider) =>
        instance.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => 
                FormsIgnorePropertyNames.Contains(x.Name) is false 
                && x.PropertyType.IsForRelatedEntityCollectionLink() is false
                && x.PropertyType.IsForEmbeddedResource() is false
                && x.PropertyType.IsForEmbeddedResourceCollection() is false)
            .Select(propertyInfo => propertyInfo.CreateFormField(instance, lookupOptionsProvider));
}