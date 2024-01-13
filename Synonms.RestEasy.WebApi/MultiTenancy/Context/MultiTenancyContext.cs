namespace Synonms.RestEasy.WebApi.MultiTenancy.Context;

public class MultiTenancyContext<TTenant>
    where TTenant : Tenant
{
    private MultiTenancyContext(TTenant tenant)
    {
        Tenant = tenant;
    }
    
    public TTenant Tenant { get; }

    public static MultiTenancyContext<TTenant> Create(TTenant tenant) =>
        new (tenant);
}