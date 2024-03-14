namespace Synonms.RestEasy.WebApi.Pipeline.Tenants.Context;

public class TenantContext<TTenant>
    where TTenant : RestEasyTenant
{
    private TenantContext(IEnumerable<TTenant> availableTenants, TTenant? selectedTenant)
    {
        AvailableTenants = availableTenants;
        SelectedTenant = selectedTenant;
    }

    public IEnumerable<TTenant> AvailableTenants { get; set; }
    
    public TTenant? SelectedTenant { get; }

    public static TenantContext<TTenant> Create(IEnumerable<TTenant> availableTenants, TTenant? selectedTenant) =>
        new (availableTenants, selectedTenant);
}