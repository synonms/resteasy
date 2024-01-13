using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record DayNumber : ValueObject<int>, IComparable, IComparable<DayNumber>
{
    private const int DefaultMinValue = 1;
    private const int DefaultMaxValue = 31;
    
    private DayNumber(int value) : base(value)
    {
    }
    
    public static implicit operator int(DayNumber valueObject) => valueObject.Value;

    public static OneOf<DayNumber, IEnumerable<DomainRuleFault>> CreateMandatory(int value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<DayNumber, IEnumerable<DomainRuleFault>> CreateMandatory(int value, int minValue, int maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(DayNumber), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(DayNumber), maxValue));
        }

        return faults.Any() ? faults : new DayNumber(value);
    }

    public static OneOf<Maybe<DayNumber>, IEnumerable<DomainRuleFault>> CreateOptional(int? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Maybe<DayNumber>, IEnumerable<DomainRuleFault>> CreateOptional(int? value, int minValue, int maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<DayNumber>, IEnumerable<DomainRuleFault>>(Maybe<DayNumber>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<DayNumber>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<DayNumber>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static DayNumber Convert(int value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new DayNumber(int.MinValue));
    
    public int CompareTo(DayNumber? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}