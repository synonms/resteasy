namespace Synonms.RestEasy.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class RestEasyOptionAttribute : Attribute
{
    public RestEasyOptionAttribute(object id, string? label, bool isEnabled = true)
    {
        Id = id;
        Label = label;
        IsEnabled = isEnabled;
    }
    
    public object Id { get; }
    
    public string? Label { get; }
    
    public bool IsEnabled { get; }
}
