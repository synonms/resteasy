using System.Text.RegularExpressions;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Forms;

namespace Synonms.RestEasy.Blazor.Validation;

public class StringInputValidator : IInputValidator
{
    public string DataType => DataTypes.String;
    
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

        List<string> validationErrors = new();

        convertedValue = input;

        if (fieldSchema.MaxLength is not null && fieldSchema.MaxLength > 0)
        {
            if (input.Trim().Length > fieldSchema.MaxLength)
            {
                validationErrors.Add($"Value can not be more than '{fieldSchema.MaxLength}' characters.");
            }
        }

        if (fieldSchema.MinLength is not null && fieldSchema.MinLength > 0)
        {
            if (input.Trim().Length < fieldSchema.MinLength)
            {
                validationErrors.Add($"Value can not be less than '{fieldSchema.MinLength}' characters.");
            }
        }

        if (string.IsNullOrWhiteSpace(fieldSchema.Pattern) is false)
        {
            Regex regex = new(fieldSchema.Pattern);
            if (regex.IsMatch(input) is false)
            {
                validationErrors.Add($"Value must match regular expression pattern '{fieldSchema.Pattern}'.");
            }
        }
        
        return validationErrors;
    }
}