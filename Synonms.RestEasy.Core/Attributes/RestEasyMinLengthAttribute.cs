namespace Synonms.RestEasy.Core.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class RestEasyMinLengthAttribute : Attribute
{
    public RestEasyMinLengthAttribute(int minLength)
    {
        MinLength = minLength;
    }

    public int MinLength { get; }
}