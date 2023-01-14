namespace Juniper.Catalyst.Rest.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CatalystMinSizeAttribute : Attribute
{
    public CatalystMinSizeAttribute(int minSize)
    {
        MinSize = minSize;
    }

    public int MinSize { get; }
}