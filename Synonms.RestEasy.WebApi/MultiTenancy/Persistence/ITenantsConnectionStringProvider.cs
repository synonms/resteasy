namespace Synonms.RestEasy.WebApi.MultiTenancy.Persistence;

/// <summary>
/// Provides the connection string to the data store containing the Tenant information.
/// Note that this is the central data store defining who the tenants are, it is NOT the database containing the HR/Payroll data for a specific Tenant. 
/// </summary>
public interface ITenantsConnectionStringProvider
{
    string Get();
}