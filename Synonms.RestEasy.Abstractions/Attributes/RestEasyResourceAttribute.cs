namespace Synonms.RestEasy.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class RestEasyResourceAttribute : Attribute
{
    public RestEasyResourceAttribute(Type resourceType, string collectionPath)
    {
        ResourceType = resourceType;
        CollectionPath = collectionPath;
    }

    public Type ResourceType { get; }
    
    public string CollectionPath { get; }
}