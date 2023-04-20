namespace Synonms.RestEasy.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class RestEasyChildResourceAttribute : Attribute
{
    public RestEasyChildResourceAttribute(Type childResourceType)
    {
        ChildResourceType = childResourceType;
    }

    public Type ChildResourceType { get; }
}