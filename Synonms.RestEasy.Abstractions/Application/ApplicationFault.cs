using Synonms.Functional;

namespace Synonms.RestEasy.Abstractions.Application;

public abstract class ApplicationFault : Fault
{
    protected ApplicationFault(string code, string title, string detail, FaultSource source, params object?[] arguments)
        : base(code, title, detail, source, arguments)
    {
    }
}