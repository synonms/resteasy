using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.Rules;

public interface IDomainRuleset
{
    IEnumerable<DomainRuleFault> Apply();
}