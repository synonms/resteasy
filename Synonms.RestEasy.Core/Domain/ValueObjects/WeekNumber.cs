using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record WeekNumber : ValueObject<int>, IComparable, IComparable<WeekNumber>
{
    private const int DefaultMinValue = 1;
    private const int DefaultMaxValue = 53;
    
    private WeekNumber(int value) : base(value)
    {
    }
    
    public static implicit operator int(WeekNumber valueObject) => valueObject.Value;
    
    public static OneOf<WeekNumber, IEnumerable<DomainRuleFault>> Create(int value, int minValue = DefaultMinValue, int maxValue = DefaultMaxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(WeekNumber), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(WeekNumber), maxValue));
        }

        return faults.Any() ? faults : new WeekNumber(value);
    }
    
    public static OneOf<Maybe<WeekNumber>, IEnumerable<DomainRuleFault>> CreateOptional(int? value, int minValue = DefaultMinValue, int maxValue = DefaultMaxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<WeekNumber>, IEnumerable<DomainRuleFault>>(Maybe<WeekNumber>.None);
        }

        return Create(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<WeekNumber>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<WeekNumber>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static WeekNumber Convert(int value) =>
        Create(value, value).Match(
            valueObject => valueObject,
            fault => new WeekNumber(int.MinValue));
    
    public int CompareTo(WeekNumber? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}