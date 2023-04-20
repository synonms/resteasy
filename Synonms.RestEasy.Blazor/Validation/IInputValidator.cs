using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Forms;

namespace Synonms.RestEasy.Blazor.Validation;

public interface IInputValidator
{
    string DataType { get; }
    
    List<string> Validate(string input, FormField fieldSchema, out object? convertedValue);
}