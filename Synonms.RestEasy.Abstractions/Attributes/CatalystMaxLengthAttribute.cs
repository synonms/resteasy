namespace Juniper.Catalyst.Rest.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CatalystMaxLengthAttribute : Attribute
{
    public CatalystMaxLengthAttribute(int maxLength)
    {
        MaxLength = maxLength;
    }

    public int MaxLength { get; }
}