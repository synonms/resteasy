namespace Juniper.Catalyst.Rest.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CatalystDescriptorAttribute : Attribute
{
    public CatalystDescriptorAttribute(string? label = null, string? placeholder = null, string? description = null, string? name = null)
    {
        Label = label;
        Placeholder = placeholder;
        Description = description;
        Name = name;
    }
    
    public string? Label { get; }
    
    public string? Placeholder { get; }

    public string? Description { get; }
    
    public string? Name { get; }
}