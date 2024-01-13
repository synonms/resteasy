using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.Rules.Rulesets;

public class ClosedDateTimeRangeRuleset : IDomainRuleset
{
    public readonly DateTime MinValue;
    public readonly DateTime MaxValue;
    
    private readonly string _fromPropertyName;
    private readonly string _toPropertyName;
    private readonly DateTime _fromValue;
    private readonly DateTime _toValue;

    private ClosedDateTimeRangeRuleset(string fromPropertyName, DateTime fromValue, string toPropertyName, DateTime toValue, DateTime minValue, DateTime maxValue)
    {
        _fromPropertyName = fromPropertyName;
        _fromValue = fromValue;
        _toPropertyName = toPropertyName;
        _toValue = toValue;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public IEnumerable<DomainRuleFault> Apply() =>
        DomainRules.CreateBuilder()
            .Property(_fromPropertyName, _fromValue)
            .AddRule(date => date < MinValue, ErrorTemplates.MinValueError, _fromPropertyName, MinValue)
            .AddRule(date => date > MaxValue, ErrorTemplates.MaxValueError, _fromPropertyName, MaxValue)
            .Property(_toPropertyName, _toValue)
            .AddRule(date => date < MinValue, ErrorTemplates.MinValueError, _toPropertyName, MinValue)
            .AddRule(date => date > MaxValue, ErrorTemplates.MaxValueError, _toPropertyName, MaxValue)
            .AddRule(date => date < _fromValue, ErrorTemplates.EndDateBeforeStartDateError, _toPropertyName, _fromValue)
            .Build();

    public static ClosedDateTimeRangeRuleset Create(string fromPropertyName, DateTime fromValue, string toPropertyName, DateTime toValue, DateTime? minDate = null, DateTime? maxDate = null) => 
        new(fromPropertyName, fromValue, toPropertyName, toValue, minDate ?? DateTime.MinValue, maxDate ?? DateTime.MaxValue);
}