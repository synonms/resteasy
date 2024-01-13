namespace Synonms.RestEasy.WebApi.MultiTenancy.Context;

public class MultiTenancyContextAccessor<TTenant> : IMultiTenancyContextAccessor<TTenant>
    where TTenant : Tenant
{
    public MultiTenancyContext<TTenant>? MultiTenancyContext { get; set; }
}