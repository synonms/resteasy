using System.Reflection;
using Synonms.RestEasy.Abstractions.Attributes;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.SharedKernel.Extensions;

namespace Synonms.RestEasy.Extensions;

public static class PropertyInfoExtensions
{
    public static FormField CreateFormField(this PropertyInfo propertyInfo, object instance, ILookupOptionsProvider lookupOptionsProvider) 
    {
        string name = propertyInfo.GetFormFieldName();

        if (propertyInfo.PropertyType.IsArrayOrEnumerable())
        {
            Type? elementType = propertyInfo.PropertyType.GetArrayOrEnumerableElementType();

            if (elementType is null)
            {
                throw new Exception($"Unable to determine element type for property '{propertyInfo.Name}' of type [{propertyInfo.PropertyType}].");
            }
            
            return new FormField(name, propertyInfo.PropertyType, elementType)
            {
                Description = propertyInfo.GetFormFieldDescription(),
                ElementType = propertyInfo.GetFormFieldElementType(),
                ElementForm = propertyInfo.GetFormFieldElementForm(lookupOptionsProvider),
                IsEnabled = propertyInfo.GetFormFieldIsEnabled(),
                IsMutable = propertyInfo.GetFormFieldIsMutable(),
                IsRequired = propertyInfo.IsRequired(),
                IsVisible = propertyInfo.GetFormFieldIsVisible(),
                Label = propertyInfo.GetFormFieldLabel(),
                MaxSize = propertyInfo.HasMaxSize(out int maxSize) ? maxSize : null,
                MinSize = propertyInfo.HasMinSize(out int minSize) ? minSize : null,
                Type = DataTypes.Array,
                Value = propertyInfo.GetFormFieldValue(instance)
            };
        }
        
        return new FormField(name, propertyInfo.PropertyType)
        {
            Description = propertyInfo.GetFormFieldDescription(),
            Form = propertyInfo.GetFormFieldForm(instance, lookupOptionsProvider),
            IsEnabled = propertyInfo.GetFormFieldIsEnabled(),
            IsMutable = propertyInfo.GetFormFieldIsMutable(),
            IsRequired = propertyInfo.IsRequired(),
            IsSecret = propertyInfo.GetFormFieldIsSecret(),
            IsVisible = propertyInfo.GetFormFieldIsVisible(),
            Label = propertyInfo.GetFormFieldLabel(),
            Max = propertyInfo.HasMax(out object max) ? max : null,
            MaxLength = propertyInfo.HasMaxLength(out int maxLength) ? maxLength : null,
            Min = propertyInfo.HasMin(out object min) ? min : null,
            MinLength = propertyInfo.HasMinLength(out int minLength) ? minLength : null,
            Options = propertyInfo.GetFormFieldOptions(lookupOptionsProvider),
            Pattern = propertyInfo.HasPattern(out string pattern) ? pattern : null,
            Placeholder = propertyInfo.GetFormFieldPlaceholder(),
            Type = propertyInfo.GetFormFieldType(),
            Value = propertyInfo.GetFormFieldValue(instance)
        };
    }
    
    public static bool HasMax<T>(this PropertyInfo propertyInfo, out T maximum)
    {
        RestEasyMaxValueAttribute? maxValueAttribute = propertyInfo.GetCustomAttribute<RestEasyMaxValueAttribute>();

        if (maxValueAttribute?.Maximum is not T maxAsT)
        {
            maximum = default!;
            return false;
        }
        
        maximum = maxAsT;
        return true;
    }
    
    public static bool HasMaxLength(this PropertyInfo propertyInfo, out int maxLength)
    {
        RestEasyMaxLengthAttribute? maxLengthAttribute = propertyInfo.GetCustomAttribute<RestEasyMaxLengthAttribute>();

        if (maxLengthAttribute is null)
        {
            maxLength = default;
            return false;
        }
        
        maxLength = maxLengthAttribute.MaxLength;
        return true;
    }
    
    public static bool HasMaxSize(this PropertyInfo propertyInfo, out int maxSize)
    {
        RestEasyMaxSizeAttribute? maxSizeAttribute = propertyInfo.GetCustomAttribute<RestEasyMaxSizeAttribute>();

        if (maxSizeAttribute is null)
        {
            maxSize = default;
            return false;
        }
        
        maxSize = maxSizeAttribute.MaxSize;
        return true;
    }
    
    public static bool HasMin<T>(this PropertyInfo propertyInfo, out T minimum)
    {
        RestEasyMinValueAttribute? minValueAttribute = propertyInfo.GetCustomAttribute<RestEasyMinValueAttribute>();

        if (minValueAttribute?.Minimum is not T minAsT)
        {
            minimum = default!;
            return false;
        }
        
        minimum = minAsT;
        return true;
    }
    
    public static bool HasMinLength(this PropertyInfo propertyInfo, out int minLength)
    {
        RestEasyMinLengthAttribute? minLengthAttribute = propertyInfo.GetCustomAttribute<RestEasyMinLengthAttribute>();

        if (minLengthAttribute is null)
        {
            minLength = default;
            return false;
        }
        
        minLength = minLengthAttribute.MinLength;
        return true;
    }
    
    public static bool HasMinSize(this PropertyInfo propertyInfo, out int minSize)
    {
        RestEasyMinSizeAttribute? minSizeAttribute = propertyInfo.GetCustomAttribute<RestEasyMinSizeAttribute>();

        if (minSizeAttribute is null)
        {
            minSize = default;
            return false;
        }
        
        minSize = minSizeAttribute.MinSize;
        return true;
    }
    
    public static bool HasPattern(this PropertyInfo propertyInfo, out string pattern)
    {
        RestEasyPatternAttribute? patternAttribute = propertyInfo.GetCustomAttribute<RestEasyPatternAttribute>();

        if (patternAttribute is null)
        {
            pattern = string.Empty;
            return false;
        }
        
        pattern = patternAttribute.Pattern;
        return true;
    }

    public static bool IsRequired(this PropertyInfo propertyInfo)
    {
        RestEasyRequiredAttribute? requiredAttribute = propertyInfo.GetCustomAttribute<RestEasyRequiredAttribute>();

        return requiredAttribute is not null;
    }

    private static string? GetFormFieldDescription(this PropertyInfo propertyInfo)
    {
        RestEasyDescriptorAttribute? descriptorAttribute = propertyInfo.GetCustomAttribute<RestEasyDescriptorAttribute>();

        return descriptorAttribute?.Description;
    }

    private static IEnumerable<FormField>? GetFormFieldElementForm(this PropertyInfo propertyInfo, ILookupOptionsProvider lookupOptionsProvider)
    {
        if (propertyInfo.PropertyType.IsArrayOrEnumerable())
        {
            Type? elementType = propertyInfo.PropertyType.GetArrayOrEnumerableElementType();
    
            if (elementType is null)
            {
                return null;
            }
    
            if (elementType.IsResource() || elementType.IsChildResource())
            {
                object? resource = Activator.CreateInstance(elementType);
    
                return resource?.GetFormFields(lookupOptionsProvider);
            }
            
            return new []
            {
                new FormField(propertyInfo.Name.ToPascalCase(), elementType)
                {
                    Max = propertyInfo.HasMax(out object max) ? max : null,
                    MaxLength = propertyInfo.HasMaxLength(out int maxLength) ? maxLength : null,
                    Min = propertyInfo.HasMin(out object min) ? min : null,
                    MinLength = propertyInfo.HasMinLength(out int minLength) ? minLength : null,
                    Options = propertyInfo.GetFormFieldOptions(lookupOptionsProvider),
                    Pattern = propertyInfo.HasPattern(out string pattern) ? pattern : null,
                    Placeholder = propertyInfo.GetFormFieldPlaceholder(),
                    Type = elementType.GetResourcePropertyType()
                }
            };
        }
    
        return null;
    }

    private static IEnumerable<FormField>? GetFormFieldForm(this PropertyInfo propertyInfo, object instance, ILookupOptionsProvider lookupOptionsProvider)
    {
        if (propertyInfo.PropertyType.IsResource() || propertyInfo.PropertyType.IsChildResource())
        {
            object? value = propertyInfo.GetValue(instance);
            
            return value?.GetFormFields(lookupOptionsProvider);
        }
    
        return null;
    }
    
    private static bool? GetFormFieldIsEnabled(this PropertyInfo propertyInfo)
    {
        RestEasyDisabledAttribute? disabledAttribute = propertyInfo.GetCustomAttribute<RestEasyDisabledAttribute>();

        return disabledAttribute is not null ? false : null;
    }

    private static bool? GetFormFieldIsMutable(this PropertyInfo propertyInfo)
    {
        RestEasyImmutableAttribute? immutableAttribute = propertyInfo.GetCustomAttribute<RestEasyImmutableAttribute>();

        return immutableAttribute is not null ? false : null;
    }

    private static bool? GetFormFieldIsSecret(this PropertyInfo propertyInfo)
    {
        RestEasySecretAttribute? secretAttribute = propertyInfo.GetCustomAttribute<RestEasySecretAttribute>();

        return secretAttribute is not null ? true : null;
    }

    private static bool? GetFormFieldIsVisible(this PropertyInfo propertyInfo)
    {
        RestEasyHiddenAttribute? hiddenAttribute = propertyInfo.GetCustomAttribute<RestEasyHiddenAttribute>();

        return hiddenAttribute is not null ? false : null;
    }

    private static string? GetFormFieldLabel(this PropertyInfo propertyInfo)
    {
        RestEasyDescriptorAttribute? descriptorAttribute = propertyInfo.GetCustomAttribute<RestEasyDescriptorAttribute>();

        return descriptorAttribute?.Label;
    }
    
    private static string GetFormFieldName(this PropertyInfo propertyInfo)
    {
        RestEasyDescriptorAttribute? descriptorAttribute = propertyInfo.GetCustomAttribute<RestEasyDescriptorAttribute>();

        return string.IsNullOrWhiteSpace(descriptorAttribute?.Name)
            ? propertyInfo.Name.ToCamelCase()
            : descriptorAttribute.Name;
    }

    private static IEnumerable<FormFieldOption>? GetFormFieldOptions(this PropertyInfo propertyInfo, ILookupOptionsProvider lookupOptionsProvider)
    {
        RestEasyLookupAttribute? lookupAttribute = propertyInfo.GetCustomAttribute<RestEasyLookupAttribute>();
        
        if (lookupAttribute is not null)
        {
            return lookupOptionsProvider.Get(lookupAttribute.Discriminator);
        }
        
        List<RestEasyOptionAttribute> optionAttributes = propertyInfo.GetCustomAttributes<RestEasyOptionAttribute>().ToList();

        if (optionAttributes.Any() is false)
        {
            return null;
        }

        List<FormFieldOption> formFieldOptions = optionAttributes.Select(x => new FormFieldOption(x.Id) { Label = x.Label, IsEnabled = x.IsEnabled }).ToList();

        return formFieldOptions;
    }
    
    private static string? GetFormFieldPlaceholder(this PropertyInfo propertyInfo)
    {
        RestEasyDescriptorAttribute? descriptorAttribute = propertyInfo.GetCustomAttribute<RestEasyDescriptorAttribute>();

        return descriptorAttribute?.Placeholder;
    }

    private static string? GetFormFieldElementType(this PropertyInfo propertyInfo)
    {
        Type? elementType = propertyInfo.PropertyType.GetArrayOrEnumerableElementType();
    
        return elementType?.GetResourcePropertyType();
    }

    private static string? GetFormFieldType(this PropertyInfo propertyInfo) =>
        propertyInfo.PropertyType.GetResourcePropertyType();

    private static object? GetFormFieldValue(this PropertyInfo propertyInfo, object instance) =>
        propertyInfo.GetValue(instance);
}