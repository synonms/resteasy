using Synonms.Functional;

namespace Synonms.RestEasy.Core.Infrastructure;

public abstract class InfrastructureFault : Fault
{
    protected InfrastructureFault(string code, string title, string detail, FaultSource source, params object?[] arguments)
        : base(code, title, detail, source, arguments)
    {
    }
}