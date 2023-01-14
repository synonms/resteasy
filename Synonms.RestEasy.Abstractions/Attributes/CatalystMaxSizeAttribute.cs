namespace Juniper.Catalyst.Rest.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CatalystMaxSizeAttribute : Attribute
{
    public CatalystMaxSizeAttribute(int maxSize)
    {
        MaxSize = maxSize;
    }

    public int MaxSize { get; }
}