using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Domain;

public interface IDomainRuleset
{
    IEnumerable<DomainRuleFault> Apply();
}