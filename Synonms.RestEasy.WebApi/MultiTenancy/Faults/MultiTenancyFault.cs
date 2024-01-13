using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.MultiTenancy.Faults;

public abstract class MultiTenancyFault : Fault
{
    protected MultiTenancyFault(string code, string detail, FaultSource source, params object?[] arguments) 
        : base(code, "MultiTenancy", detail, source, arguments)
    {
    }
}