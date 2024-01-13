using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Months : ValueObject<int>, IComparable, IComparable<Months>
{
    private const int DefaultMinValue = 0;
    private const int DefaultMaxValue = int.MaxValue;

    private Months(int value) : base(value)
    {
    }
    
    public static implicit operator int(Months valueObject) => valueObject.Value;

    public static OneOf<Months, IEnumerable<DomainRuleFault>> CreateMandatory(int value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Months, IEnumerable<DomainRuleFault>> CreateMandatory(int value, int minValue, int maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(Months), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(Months), maxValue));
        }

        return faults.Any() ? faults : new Months(value);
    }

    public static OneOf<Maybe<Months>, IEnumerable<DomainRuleFault>> CreateOptional(int? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Maybe<Months>, IEnumerable<DomainRuleFault>> CreateOptional(int? value, int minValue, int maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<Months>, IEnumerable<DomainRuleFault>>(Maybe<Months>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<Months>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Months>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Months Convert(int value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new Months(int.MinValue)
        );
    
    public int CompareTo(Months? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}