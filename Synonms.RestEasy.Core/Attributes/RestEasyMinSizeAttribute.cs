namespace Synonms.RestEasy.Core.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class RestEasyMinSizeAttribute : Attribute
{
    public RestEasyMinSizeAttribute(int minSize)
    {
        MinSize = minSize;
    }

    public int MinSize { get; }
}