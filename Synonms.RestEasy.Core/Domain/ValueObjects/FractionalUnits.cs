using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record FractionalUnits : ValueObject<decimal>, IComparable, IComparable<FractionalUnits>
{
    private const decimal DefaultMinValue = 0.0m;
    private const decimal DefaultMaxValue = decimal.MaxValue;

    private FractionalUnits(decimal value) : base(value)
    {
    }
    
    public static implicit operator decimal(FractionalUnits valueObject) => valueObject.Value;

    public static OneOf<FractionalUnits, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<FractionalUnits, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value, decimal minValue, decimal maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(FractionalUnits), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(FractionalUnits), maxValue));
        }

        return faults.Any() ? faults : new FractionalUnits(value);
    }

    public static OneOf<Maybe<FractionalUnits>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Maybe<FractionalUnits>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value, decimal minValue, decimal maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<FractionalUnits>, IEnumerable<DomainRuleFault>>(Maybe<FractionalUnits>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<FractionalUnits>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<FractionalUnits>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static FractionalUnits Convert(decimal value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new FractionalUnits(decimal.MinValue)
        );
    
    public int CompareTo(FractionalUnits? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}