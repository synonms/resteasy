using Synonms.Functional;

namespace Synonms.RestEasy.Core.Domain.Faults;

public class DomainRulesFault : DomainFault
{
    public DomainRulesFault(IEnumerable<DomainRuleFault> faults) 
        : base(nameof(DomainRulesFault), "Domain rules", string.Join("\r\n", faults), new FaultSource())
    {
        Faults = faults;
    }

    public IEnumerable<DomainRuleFault> Faults { get; }
}