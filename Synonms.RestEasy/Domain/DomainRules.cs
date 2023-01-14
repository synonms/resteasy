using Synonms.Functional;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Domain;

public static class DomainRules
{
    public static DomainRulesBuilder CreateBuilder() => new ();

    public static Maybe<Fault> Apply(params IDomainRuleset[] rulesets)
    {
        List<DomainRuleFault> faults = rulesets.SelectMany(x => x.Apply()).ToList();

        return faults.Any()
            ? new DomainRulesFault(faults)
            : Maybe<Fault>.None;
    }
}