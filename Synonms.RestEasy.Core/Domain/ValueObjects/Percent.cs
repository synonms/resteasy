using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Percent : ValueObject<decimal>, IComparable, IComparable<Percent>
{
    private const decimal DefaultMinValue = 0.0m;
    private const decimal DefaultMaxValue = 100.0m;
    
    private Percent(decimal value) : base(value)
    {
    }
    
    public static implicit operator decimal(Percent valueObject) => valueObject.Value;

    public static OneOf<Percent, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);
    
    public static OneOf<Percent, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value, decimal minValue, decimal maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(Percent), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(Percent), maxValue));
        }

        return faults.Any() ? faults : new Percent(value);
    }

    public static OneOf<Maybe<Percent>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Maybe<Percent>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value, decimal minValue, decimal maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<Percent>, IEnumerable<DomainRuleFault>>(Maybe<Percent>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<Percent>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Percent>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Percent Convert(decimal value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new Percent(decimal.MinValue)
        );
    
    public int CompareTo(Percent? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}