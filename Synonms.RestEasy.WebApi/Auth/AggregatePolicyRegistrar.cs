using System.Reflection;
using Synonms.RestEasy.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Synonms.RestEasy.Core.Attributes;

namespace Synonms.RestEasy.WebApi.Auth;

public abstract class AggregatePolicyRegistrar<TAggregateRoot> : IPolicyRegistrar
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    protected string PermissionName;
    protected string CreatePermission;
    protected string ReadPermission;
    protected string UpdatePermission;
    protected string DeletePermission;
    protected string AuthPolicyName;
    protected string CreatePolicy;
    protected string ReadPolicy;
    protected string UpdatePolicy;
    protected string DeletePolicy;

    protected AggregatePolicyRegistrar()
    {
        RestEasyResourceAttribute? resourceAttribute = typeof(TAggregateRoot).GetCustomAttribute<RestEasyResourceAttribute>();

        PermissionName = resourceAttribute?.CollectionPath ?? string.Empty;

        CreatePermission = Permissions.CreatePrefix + PermissionName;
        ReadPermission = Permissions.ReadPrefix + PermissionName;
        UpdatePermission = Permissions.UpdatePrefix + PermissionName;
        DeletePermission = Permissions.DeletePrefix + PermissionName;

        AuthPolicyName = typeof(TAggregateRoot).Name;

        CreatePolicy = Policies.CreatePrefix + AuthPolicyName;
        ReadPolicy = Policies.ReadPrefix + AuthPolicyName;
        UpdatePolicy = Policies.UpdatePrefix + AuthPolicyName;
        DeletePolicy = Policies.DeletePrefix + AuthPolicyName;
    }

    public virtual void Register(AuthorizationOptions authorisationOptions)
    {
        RequireDefaultPermissionsClaims(authorisationOptions);
    }

    protected void RequireDefaultPermissionsClaims(AuthorizationOptions authorisationOptions)
    {
        if (string.IsNullOrWhiteSpace(AuthPolicyName))
        {
            return;
        }
        
        authorisationOptions.AddPolicy(CreatePolicy, policy => policy.RequireClaim(Permissions.ClaimType, CreatePermission));
        authorisationOptions.AddPolicy(ReadPolicy, policy => policy.RequireClaim(Permissions.ClaimType, ReadPermission));
        authorisationOptions.AddPolicy(UpdatePolicy, policy => policy.RequireClaim(Permissions.ClaimType, UpdatePermission));
        authorisationOptions.AddPolicy(DeletePolicy, policy => policy.RequireClaim(Permissions.ClaimType, DeletePermission));
    }
    
    protected void AllowAll(AuthorizationOptions authorisationOptions)
    {
        if (string.IsNullOrWhiteSpace(AuthPolicyName))
        {
            return;
        }
        
        authorisationOptions.AddPolicy(CreatePolicy, _ => {});
        authorisationOptions.AddPolicy(ReadPolicy, _ => {});
        authorisationOptions.AddPolicy(UpdatePolicy, _ => {});
        authorisationOptions.AddPolicy(DeletePolicy, _ => {});
    }
}