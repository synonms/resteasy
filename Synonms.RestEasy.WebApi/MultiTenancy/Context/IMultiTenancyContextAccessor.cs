namespace Synonms.RestEasy.WebApi.MultiTenancy.Context;

public interface IMultiTenancyContextAccessor<TTenant>
    where TTenant : Tenant
{
    MultiTenancyContext<TTenant>? MultiTenancyContext { get; set; } 
}