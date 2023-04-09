using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Blazor.Validation;

public interface IInputValidator
{
    string DataType { get; }
    
    List<string> Validate(string input, FormField fieldSchema, out object? convertedValue);
}