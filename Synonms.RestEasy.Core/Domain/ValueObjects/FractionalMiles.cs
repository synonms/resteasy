using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record FractionalMiles : ValueObject<decimal>, IComparable, IComparable<FractionalMiles>
{
    private const decimal DefaultMinValue = 0.0m;
    private const decimal DefaultMaxValue = decimal.MaxValue;

    private FractionalMiles(decimal value) : base(value)
    {
    }
    
    public static implicit operator decimal(FractionalMiles valueObject) => valueObject.Value;

    public static OneOf<FractionalMiles, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<FractionalMiles, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value, decimal minValue, decimal maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(FractionalMiles), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(FractionalMiles), maxValue));
        }

        return faults.Any() ? faults : new FractionalMiles(value);
    }

    public static OneOf<Maybe<FractionalMiles>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Maybe<FractionalMiles>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value, decimal minValue, decimal maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<FractionalMiles>, IEnumerable<DomainRuleFault>>(Maybe<FractionalMiles>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<FractionalMiles>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<FractionalMiles>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static FractionalMiles Convert(decimal value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new FractionalMiles(decimal.MinValue)
        );
    
    public int CompareTo(FractionalMiles? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}