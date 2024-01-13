using Synonms.Functional;

namespace Synonms.RestEasy.Core.Application.Faults;

public abstract class ApplicationFault : Fault
{
    protected ApplicationFault(string code, string title, string detail, FaultSource source, params object?[] arguments)
        : base(code, title, detail, source, arguments)
    {
    }
}