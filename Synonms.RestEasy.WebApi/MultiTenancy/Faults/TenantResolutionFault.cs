using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.MultiTenancy.Faults;

public class TenantResolutionFault : MultiTenancyFault
{
    public TenantResolutionFault(string detail, params object?[] arguments) 
        : this(detail, new FaultSource(), arguments)
    {
    }

    public TenantResolutionFault(string detail, FaultSource source, params object?[] arguments) 
        : base(nameof(TenantResolutionFault), detail, source, arguments)
    {
    }
}