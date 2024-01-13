using Synonms.RestEasy.WebApi.Schema;

namespace Synonms.RestEasy.WebApi.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class RestEasyResourceAttribute : Attribute
{
    public RestEasyResourceAttribute(Type resourceType, string collectionPath, bool requiresAuthentication, string? authorisationPolicyName = null, int pageLimit = Pagination.DefaultPageLimit, bool isCreateDisabled = false, bool isUpdateDisabled = false, bool isDeleteDisabled = false)
    {
        ResourceType = resourceType;
        CollectionPath = collectionPath;
        RequiresAuthentication = requiresAuthentication;
        AuthorisationPolicyName = authorisationPolicyName;
        PageLimit = pageLimit < 1 ? Pagination.DefaultPageLimit : pageLimit;
        IsCreateDisabled = isCreateDisabled;
        IsUpdateDisabled = isUpdateDisabled;
        IsDeleteDisabled = isDeleteDisabled;
    }

    public Type ResourceType { get; }
    
    public string CollectionPath { get; }
    
    public bool RequiresAuthentication { get; }

    public string? AuthorisationPolicyName { get; }
    
    public int PageLimit { get; }

    public bool IsCreateDisabled { get; }
    
    public bool IsUpdateDisabled { get; }
    
    public bool IsDeleteDisabled { get; }
}