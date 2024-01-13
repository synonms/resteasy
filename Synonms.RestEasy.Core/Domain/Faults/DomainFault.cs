using Synonms.Functional;

namespace Synonms.RestEasy.Core.Domain.Faults;

public abstract class DomainFault : Fault
{
    protected DomainFault(string code, string title, string detail, FaultSource source, params object?[] arguments)
        : base(code, title, detail, source, arguments)
    {
    }
}