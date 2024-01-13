using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record FractionalHours : ValueObject<decimal>, IComparable, IComparable<FractionalHours>
{
    private const decimal DefaultMinValue = 0.0m;
    private const decimal DefaultMaxValue = decimal.MaxValue;

    private FractionalHours(decimal value) : base(value)
    {
    }
    
    public static implicit operator decimal(FractionalHours valueObject) => valueObject.Value;

    public static OneOf<FractionalHours, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<FractionalHours, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value, decimal minValue, decimal maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(FractionalHours), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(FractionalHours), maxValue));
        }

        return faults.Any() ? faults : new FractionalHours(value);
    }

    public static OneOf<Maybe<FractionalHours>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Maybe<FractionalHours>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value, decimal minValue, decimal maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<FractionalHours>, IEnumerable<DomainRuleFault>>(Maybe<FractionalHours>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<FractionalHours>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<FractionalHours>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static FractionalHours Convert(decimal value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new FractionalHours(decimal.MinValue)
        );
    
    public int CompareTo(FractionalHours? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}