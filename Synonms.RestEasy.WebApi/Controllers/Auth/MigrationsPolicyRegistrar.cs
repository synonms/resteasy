using Microsoft.AspNetCore.Authorization;
using Synonms.RestEasy.WebApi.Auth;

namespace Synonms.RestEasy.WebApi.Controllers.Auth;

public class MigrationsPolicyRegistrar : IPolicyRegistrar
{
    public const string PermissionName = "migration";
    public const string PolicyName = "Migrations";

    public const string ReadPermission = PermissionName + "." + Permissions.Read;
    public const string CreatePermission = PermissionName + "." + Permissions.Create;
    
    public const string CreatePolicy = Policies.CreatePrefix + PolicyName;
    public const string ReadPolicy = Policies.ReadPrefix + PolicyName;

    public void Register(AuthorizationOptions authorisationOptions)
    {
        authorisationOptions.AddPolicy(ReadPolicy, policy => policy.RequireClaim(Permissions.ClaimType, ReadPermission));
        authorisationOptions.AddPolicy(CreatePolicy, policy => policy.RequireClaim(Permissions.ClaimType, CreatePermission));
    }
}