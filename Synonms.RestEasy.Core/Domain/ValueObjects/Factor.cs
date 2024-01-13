using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Factor : ValueObject<decimal>, IComparable, IComparable<Factor>
{
    private const decimal DefaultMinValue = 0.0m;
    private const decimal DefaultMaxValue = 3.0m;
    
    private Factor(decimal value) : base(value)
    {
    }
    
    public static implicit operator decimal(Factor valueObject) => valueObject.Value;

    public static OneOf<Factor, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);
    
    public static OneOf<Factor, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value, decimal minValue, decimal maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(Factor), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(Factor), maxValue));
        }

        return faults.Any() ? faults : new Factor(value);
    }
    
    public static OneOf<Maybe<Factor>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);
    
    public static OneOf<Maybe<Factor>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value, decimal minValue, decimal maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<Factor>, IEnumerable<DomainRuleFault>>(Maybe<Factor>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<Factor>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Factor>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Factor Convert(decimal value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new Factor(decimal.MinValue)
        );
    
    public int CompareTo(Factor? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}