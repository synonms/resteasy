namespace Synonms.RestEasy.WebApi.MultiTenancy.Persistence;

/// <summary>
/// Provides the connection string to the data store containing the RestEasyTenant information.
/// Note that this is the central data store defining who the tenants are, it is NOT the database containing the product data for a specific RestEasyTenant. 
/// </summary>
public interface ITenantsConnectionStringProvider
{
    string Get();
}