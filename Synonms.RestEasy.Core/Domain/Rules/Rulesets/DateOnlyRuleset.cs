using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.Rules.Rulesets;

public class DateOnlyRuleset : IDomainRuleset
{
    private const string PropertyMinValueError = "{0} must be {1} or more.";
    private const string PropertyMaxValueError = "{0} must be {1} or less.";
    
    public readonly DateOnly MinValue;
    public readonly DateOnly MaxValue;
    
    private readonly string _propertyName;
    private readonly DateOnly _value;

    private DateOnlyRuleset(string propertyName, DateOnly value, DateOnly minDate, DateOnly maxDate)
    {
        _propertyName = propertyName;
        _value = value;
        MinValue = minDate;
        MaxValue = maxDate;
    }

    public IEnumerable<DomainRuleFault> Apply() =>
        DomainRules.CreateBuilder()
            .Property(_propertyName, _value)
            .AddRule(date => date < MinValue, PropertyMinValueError, _propertyName, MinValue)
            .AddRule(date => date > MaxValue, PropertyMaxValueError, _propertyName, MaxValue)
            .Build();

    public static DateOnlyRuleset Create(string propertyName, DateOnly value, DateOnly? minDate = null, DateOnly? maxDate = null) => 
        new (propertyName, value, minDate ?? DateOnly.MinValue, maxDate ?? DateOnly.MaxValue);
}