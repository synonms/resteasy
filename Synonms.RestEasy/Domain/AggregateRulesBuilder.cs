using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Domain;

public class AggregateRulesBuilder
{
    private readonly List<DomainRuleFault> _faults = new();

    public AggregateRulesBuilder WithMandatoryValueObject<TValue, TValueObject>(TValue value, Func<TValue, OneOf<TValueObject, IEnumerable<DomainRuleFault>>> createFunc, out TValueObject valueObject)
        where TValueObject : ValueObject<TValue>
    {
        TValueObject output = default(TValueObject)!;

        createFunc.Invoke(value).Match(
            createdValueObject => output = createdValueObject,
            faults => _faults.AddRange(faults));

        valueObject = output;

        return this;
    }

    public AggregateRulesBuilder WithOptionalValueObject<TValue, TValueObject>(TValue? value, Func<TValue?, OneOf<Maybe<TValueObject>, IEnumerable<DomainRuleFault>>> createFunc, out TValueObject? valueObject)
        where TValueObject : ValueObject<TValue>
    {
        TValueObject? output = null;

        createFunc.Invoke(value).Match(
            maybeValueObject => output = maybeValueObject.Match(valueObject => valueObject, () => null as TValueObject),
            faults => _faults.AddRange(faults));

        valueObject = output;

        return this;
    }

    public AggregateRulesBuilder WithDomainRules(params IDomainRuleset[] rulesets)
    {
        _faults.AddRange(rulesets.SelectMany(x => x.Apply()).ToList());
        
        return this;
    }

    public Maybe<Fault> Build() =>
        _faults.Any() ? new DomainRulesFault(_faults) : Maybe<Fault>.None;
}