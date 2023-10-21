using System.Reflection;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Forms;
using Synonms.RestEasy.Blazor.Validation;
using Synonms.RestEasy.SharedKernel.Extensions;

namespace Synonms.RestEasy.Blazor.Components.Forms;

public class RestEasyFormInputDefinition<TModel>
{
    private string _value;
    private readonly FormField _fieldSchema;
    
    public event EventHandler? ValueChanged;

    public RestEasyFormInputDefinition(string formHtmlId, PropertyInfo propertyInfo, FormField fieldSchema, string initialValue)
    {
        PropertyInfo = propertyInfo;
        _fieldSchema = fieldSchema;
        _value = initialValue;
        
        HtmlInputId = formHtmlId + '_' + propertyInfo.Name;
    }

    public PropertyInfo PropertyInfo { get; }
    
    /// <summary>
    /// Value to bind to the input control
    /// </summary>
    public string Value
    {
        get => _value;
        set
        {
            _value = value;

            TryValidate(out _);
            
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public List<string> ValidationErrors { get; private set; } = new();

    /// <summary>
    /// Type for the HTML element
    /// </summary>
    public string HtmlInputType => GetHtmlInputType();

    /// <summary>
    /// Id for the HTML element
    /// </summary>
    public string HtmlInputId { get; }
    
    /// <summary>
    /// Placeholder for the control
    /// </summary>
    public string HtmlInputPlaceholder => _fieldSchema.Placeholder ?? string.Empty; 

    /// <summary>
    /// Disabled flag
    /// </summary>
    public string? HtmlInputDisabled => _fieldSchema.IsEnabled ?? true ? null : "disabled"; 

    /// <summary>
    /// ReadOnly flag
    /// </summary>
    public string? HtmlInputReadOnly => _fieldSchema.IsMutable ?? true ? null : "readonly"; 

    /// <summary>
    /// Display text for the label
    /// </summary>
    public string HtmlLabelText => (_fieldSchema.Label ?? _fieldSchema.Name.ToPascalCase()) + (_fieldSchema.IsRequired ?? false ? " *" : string.Empty);
    
    public IEnumerable<FormFieldOption>? HtmlSelectOptions => _fieldSchema.Options;
    
    public bool TryValidate(out object? validValue)
    {
        IInputValidator? validator = InputValidatorFactory.Create(_fieldSchema.Type ?? string.Empty);

        if (validator is null)
        {
            ValidationErrors.Clear();
            ValidationErrors.Add($"Unable to obtain validator for data type '{_fieldSchema.Type}'.");

            validValue = null;
            return false;
        }

        ValidationErrors = validator.Validate(_value, _fieldSchema, out validValue);

        return ValidationErrors.Any() is false;
    }

    public static List<RestEasyFormInputDefinition<TModel>> Create(RestEasyForm<TModel> form, Form formSchema)
    {
        List<RestEasyFormInputDefinition<TModel>> definitions = new();

        foreach (PropertyInfo propertyInfo in typeof(TModel).GetProperties())
        {
            if (propertyInfo.Name.Equals(nameof(Resource.Id)))
            {
                continue;
            }
            
            FormField? fieldSchema = formSchema.Fields.SingleOrDefault(x => string.Equals(x.Name, propertyInfo.Name, StringComparison.OrdinalIgnoreCase));

            if (fieldSchema is null)
            {
                continue;
            }

            string initialValue = fieldSchema.Value?.ToString() ?? string.Empty;
            
            RestEasyFormInputDefinition<TModel> definition = new(form.HtmlId, propertyInfo, fieldSchema, initialValue);
            
            definitions.Add(definition);
        }

        return definitions;
    }
    
    private string GetHtmlInputType() =>
        _fieldSchema.Type switch
        {
            _ when _fieldSchema.IsVisible is false => "hidden",
            _ when _fieldSchema.IsSecret is false => "password",
            DataTypes.Boolean => "checkbox",
            DataTypes.DateOnly => "date",
            DataTypes.DateTime => "datetime-local",
            DataTypes.Decimal => "number",
            DataTypes.Integer => "number",
            DataTypes.Number => "number",
            DataTypes.String => "text",
            DataTypes.TimeOnly => "time",
            
//            DataTypes.Array => :
//            DataTypes.Duration => :
//            DataTypes.Object => :
            _ => throw new NotSupportedException($"Model property type {_fieldSchema.Type} not supported.")
        };
    
//     private bool TryConvert(out object? convertedValue)
//     {
//         convertedValue = null;
//         
//         switch (_fieldSchema.Type)
//         {
//             case DataTypes.Boolean:
//             {
//                 if (bool.TryParse(_value, out bool convertedBool) is false)
//                 {
//                     return false;
//                 }
//                 
//                 convertedValue = convertedBool;
//                 return true;
//             }
//             case DataTypes.DateOnly:
//             {
//                 if (DateOnly.TryParse(_value, out DateOnly convertedDateOnly) is false)
//                 {
//                     return false;
//                 }
//                 
//                 convertedValue = convertedDateOnly;
//                 return true;
//             }
//             case DataTypes.DateTime:
//             {
//                 if (DateTime.TryParse(_value, out DateTime convertedDateTime) is false)
//                 {
//                     return false;
//                 }
//                 
//                 convertedValue = convertedDateTime;
//                 return true;
//             }
//             case DataTypes.Decimal:
//             {
//                 if (decimal.TryParse(_value, out decimal convertedDecimal) is false)
//                 {
//                     return false;
//                 }
//                 
//                 convertedValue = convertedDecimal;
//                 return true;
//             }
//             case DataTypes.Integer:
//             {
//                 if (int.TryParse(_value, out int convertedInt) is false)
//                 {
//                     return false;
//                 }
//                 
//                 convertedValue = convertedInt;
//                 return true;
//             }
//             case DataTypes.Number:
//             {
//                 if (double.TryParse(_value, out double convertedDouble) is false)
//                 {
//                     return false;
//                 }
//                 
//                 convertedValue = convertedDouble;
//                 return true;
//             }
//             case DataTypes.String:
//             {
//                 convertedValue = _value;
//                 return true;
//             }
//             case DataTypes.TimeOnly:
//             {
//                 if (TimeOnly.TryParse(_value, out TimeOnly convertedTimeOnly) is false)
//                 {
//                     return false;
//                 }
//                 
//                 convertedValue = convertedTimeOnly;
//                 return true;
//             }
//
// //            DataTypes.Array => :
// //            DataTypes.Duration => :
// //            DataTypes.Object => :
//             default:
//                 throw new NotSupportedException($"Model property type {_fieldSchema.Type} not supported.");
//         };
//    }
}