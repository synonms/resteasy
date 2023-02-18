using System.Reflection;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;

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
            .Where(x => FormsIgnorePropertyNames.Contains(x.Name) is false)
            .Select(propertyInfo => propertyInfo.CreateFormField(instance, lookupOptionsProvider));
}