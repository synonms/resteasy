namespace Synonms.RestEasy.Core.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class RestEasyMaxLengthAttribute : Attribute
{
    public RestEasyMaxLengthAttribute(int maxLength)
    {
        MaxLength = maxLength;
    }

    public int MaxLength { get; }
}