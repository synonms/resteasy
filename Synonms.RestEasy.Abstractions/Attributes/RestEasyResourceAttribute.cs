namespace Synonms.RestEasy.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class RestEasyResourceAttribute : Attribute
{
    public RestEasyResourceAttribute(string collectionPath)
    {
        CollectionPath = collectionPath;
    }
    
    public string CollectionPath { get; }
}