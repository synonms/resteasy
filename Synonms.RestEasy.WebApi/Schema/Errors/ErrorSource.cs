namespace Synonms.RestEasy.WebApi.Schema.Errors;

public class ErrorSource
{
    public ErrorSource(string? pointer = null, string? parameter = null)
    {
        Pointer = pointer;
        Parameter = parameter;
    }

    public string? Pointer { get; }

    public string? Parameter { get; }
}