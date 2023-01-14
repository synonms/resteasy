namespace Juniper.Catalyst.Rest.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CatalystMinLengthAttribute : Attribute
{
    public CatalystMinLengthAttribute(int minLength)
    {
        MinLength = minLength;
    }

    public int MinLength { get; }
}
