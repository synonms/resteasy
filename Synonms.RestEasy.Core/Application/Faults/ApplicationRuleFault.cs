using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Application.Faults;

public class ApplicationRuleFault : DomainFault
{
    public ApplicationRuleFault(string detail, params object?[] arguments)
        : this(detail, new FaultSource(), arguments)
    {
    }

    public ApplicationRuleFault(string detail, FaultSource source, params object?[] arguments)
        : base(nameof(ApplicationRuleFault), "Application rule", detail, source, arguments)
    {
    }
}