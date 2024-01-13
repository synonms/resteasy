using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.Rules.Rulesets;

public class EffectiveFromDateOnlyRuleset : IDomainRuleset
{
    private readonly string _propertyName;
    private readonly DateOnly _value;
    private readonly DateOnly _minValue;
    private readonly DateOnly _maxValue;

    private EffectiveFromDateOnlyRuleset(string propertyName, DateOnly value, DateOnly? minValue, DateOnly? maxValue)
    {
        _propertyName = propertyName;
        _value = value;
        _minValue = minValue ?? DateOnly.MinValue.AddDays(1);
        _maxValue = maxValue ?? DateOnly.MaxValue.AddDays(-1);
    }

    public IEnumerable<DomainRuleFault> Apply() =>
        DomainRules.CreateBuilder()
            .Property(_propertyName, _value)
            .AddRule(effectiveFrom => effectiveFrom < _minValue, ErrorTemplates.MinValueError, _propertyName, _minValue)
            .AddRule(effectiveFrom => effectiveFrom > _maxValue, ErrorTemplates.MaxValueError, _propertyName, _maxValue)
            .Build();

    public static EffectiveFromDateOnlyRuleset Create(string propertyName, DateOnly value, DateOnly? minValue = null, DateOnly? maxValue = null) => new (propertyName, value, minValue, maxValue);
}