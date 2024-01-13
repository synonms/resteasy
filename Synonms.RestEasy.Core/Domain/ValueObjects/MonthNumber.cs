using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record MonthNumber : ValueObject<int>, IComparable, IComparable<MonthNumber>
{
    private const int DefaultMinValue = 1;
    private const int DefaultMaxValue = 12;
    
    private MonthNumber(int value) : base(value)
    {
    }
    
    public static implicit operator int(MonthNumber valueObject) => valueObject.Value;

    public static OneOf<MonthNumber, IEnumerable<DomainRuleFault>> CreateMandatory(int value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<MonthNumber, IEnumerable<DomainRuleFault>> CreateMandatory(int value, int minValue, int maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(MonthNumber), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(MonthNumber), maxValue));
        }

        return faults.Any() ? faults : new MonthNumber(value);
    }

    public static OneOf<Maybe<MonthNumber>, IEnumerable<DomainRuleFault>> CreateOptional(int? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Maybe<MonthNumber>, IEnumerable<DomainRuleFault>> CreateOptional(int? value, int minValue, int maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<MonthNumber>, IEnumerable<DomainRuleFault>>(Maybe<MonthNumber>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<MonthNumber>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<MonthNumber>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static MonthNumber Convert(int value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new MonthNumber(int.MinValue));
    
    public int CompareTo(MonthNumber? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}