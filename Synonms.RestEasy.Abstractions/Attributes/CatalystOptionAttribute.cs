namespace Juniper.Catalyst.Rest.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class CatalystOptionAttribute : Attribute
{
    public CatalystOptionAttribute(object id, string? label, bool isEnabled = true)
    {
        Id = id;
        Label = label;
        IsEnabled = isEnabled;
    }
    
    public object Id { get; }
    
    public string? Label { get; }
    
    public bool IsEnabled { get; }
}
