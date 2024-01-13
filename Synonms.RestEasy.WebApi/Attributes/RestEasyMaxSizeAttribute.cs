namespace Synonms.RestEasy.WebApi.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class RestEasyMaxSizeAttribute : Attribute
{
    public RestEasyMaxSizeAttribute(int maxSize)
    {
        MaxSize = maxSize;
    }

    public int MaxSize { get; }
}