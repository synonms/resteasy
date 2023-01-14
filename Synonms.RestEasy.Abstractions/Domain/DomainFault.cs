using Synonms.Functional;

namespace Synonms.RestEasy.Abstractions.Domain;

public abstract class DomainFault : Fault
{
    protected DomainFault(string code, string title, string detail, FaultSource source, params object?[] arguments)
        : base(code, title, detail, source, arguments)
    {
    }
}