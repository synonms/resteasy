using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record FractionalDays : ValueObject<decimal>, IComparable, IComparable<FractionalDays>
{
    private const decimal DefaultMinValue = 0.0m;
    private const decimal DefaultMaxValue = decimal.MaxValue;

    private FractionalDays(decimal value) : base(value)
    {
    }
    
    public static implicit operator decimal(FractionalDays valueObject) => valueObject.Value;

    public static OneOf<FractionalDays, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<FractionalDays, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value, decimal minValue, decimal maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(FractionalDays), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(FractionalDays), maxValue));
        }

        return faults.Any() ? faults : new FractionalDays(value);
    }

    public static OneOf<Maybe<FractionalDays>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Maybe<FractionalDays>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value, decimal minValue, decimal maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<FractionalDays>, IEnumerable<DomainRuleFault>>(Maybe<FractionalDays>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<FractionalDays>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<FractionalDays>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static FractionalDays Convert(decimal value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new FractionalDays(decimal.MinValue)
        );
    
    public int CompareTo(FractionalDays? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}