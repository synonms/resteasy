using Synonms.RestEasy.Core.Schema;

namespace Synonms.RestEasy.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RestEasyResourceAttribute : Attribute
{
    public RestEasyResourceAttribute(Type resourceType, string collectionPath, bool allowAnonymous = false, int pageLimit = Pagination.DefaultPageLimit, bool isCreateDisabled = false, bool isUpdateDisabled = false, bool isDeleteDisabled = false)
    {
        ResourceType = resourceType;
        CollectionPath = collectionPath;
        AllowAnonymous = allowAnonymous;
        PageLimit = pageLimit < 1 ? Pagination.DefaultPageLimit : pageLimit;
        IsCreateDisabled = isCreateDisabled;
        IsUpdateDisabled = isUpdateDisabled;
        IsDeleteDisabled = isDeleteDisabled;
    }

    public Type ResourceType { get; }
    
    public string CollectionPath { get; }
    
    public bool AllowAnonymous { get; }

    public int PageLimit { get; }

    public bool IsCreateDisabled { get; }
    
    public bool IsUpdateDisabled { get; }
    
    public bool IsDeleteDisabled { get; }
}