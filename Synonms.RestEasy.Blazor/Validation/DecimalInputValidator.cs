using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Forms;

namespace Synonms.RestEasy.Blazor.Validation;

public class DecimalInputValidator : IInputValidator
{
    public string DataType => DataTypes.Decimal;

    public List<string> Validate(string input, FormField fieldSchema, out object? convertedValue)
    {
        bool isRequired = fieldSchema.IsRequired ?? false;
        
        if (isRequired && string.IsNullOrWhiteSpace(input))
        {
            convertedValue = null;
            return new List<string>
            {
                "Field is required."
            };
        }

        if (decimal.TryParse(input, out decimal convertedInput) is false)
        {
            convertedValue = null;
            return new List<string>
            {
                $"Failed to convert input to target type {typeof(decimal)}."
            };
        }

        convertedValue = convertedInput;

        List<string> validationErrors = new();
        
        if (fieldSchema.Max is not null && decimal.TryParse(fieldSchema.Max.ToString(), out decimal max))
        {
            if (convertedInput > max)
            {
                validationErrors.Add($"Value can not be greater than '{max}'.");
            }
        }

        if (fieldSchema.Min is not null && decimal.TryParse(fieldSchema.Min.ToString(), out decimal min))
        {
            if (convertedInput < min)
            {
                validationErrors.Add($"Value can not be less than '{min}'.");
            }
        }

        return validationErrors;
    }
}