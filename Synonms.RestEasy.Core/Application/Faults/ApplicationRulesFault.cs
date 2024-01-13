using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Application.Faults;

public class ApplicationRulesFault : DomainFault
{
    public ApplicationRulesFault(IEnumerable<ApplicationRuleFault> faults) 
        : base(nameof(ApplicationRulesFault), "Application rules", string.Join("\r\n", faults), new FaultSource())
    {
        Faults = faults;
    }

    public IEnumerable<ApplicationRuleFault> Faults { get; }
}