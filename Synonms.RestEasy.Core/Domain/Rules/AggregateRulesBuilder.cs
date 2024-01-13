using Ardalis.SmartEnum;
using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.Rules;

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
        where TValue : struct
        where TValueObject : ValueObject<TValue>
    {
        TValueObject? output = null;

        createFunc.Invoke(value).Match(
            maybeValueObject => output = maybeValueObject.Match(valueObject => valueObject, () => null as TValueObject),
            faults => _faults.AddRange(faults));

        valueObject = output;

        return this;
    }

    public AggregateRulesBuilder WithOptionalValueObject<TValue, TValueObject>(TValue? value, Func<TValue?, OneOf<Maybe<TValueObject>, IEnumerable<DomainRuleFault>>> createFunc, out TValueObject? valueObject)
        where TValue : class
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

    public AggregateRulesBuilder WithSmartEnum<TSmartEnum, TValue>(TValue value, out TSmartEnum smartEnum) 
        where TSmartEnum : SmartEnum<TSmartEnum, TValue>
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        TSmartEnum output = default(TSmartEnum)!;

        try
        {
            output = SmartEnum<TSmartEnum, TValue>.FromValue(value);
        }
        catch (SmartEnumNotFoundException)
        {
            DomainRuleFault fault = new("SmartEnum type {0} with value of '{1}' not found.", nameof(TSmartEnum), value);
            _faults.Add(fault);
        }

        smartEnum = output;

        return this;
    }
    
    public Maybe<Fault> Build() =>
        _faults.Any() ? new DomainRulesFault(_faults) : Maybe<Fault>.None;
}