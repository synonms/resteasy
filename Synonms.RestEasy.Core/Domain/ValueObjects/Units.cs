using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Units : ValueObject<int>, IComparable, IComparable<Units>
{
    private const int DefaultMinValue = 0;
    private const int DefaultMaxValue = int.MaxValue;

    private Units(int value) : base(value)
    {
    }
    
    public static implicit operator int(Units valueObject) => valueObject.Value;

    public static OneOf<Units, IEnumerable<DomainRuleFault>> CreateMandatory(int value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Units, IEnumerable<DomainRuleFault>> CreateMandatory(int value, int minValue, int maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(Units), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(Units), maxValue));
        }

        return faults.Any() ? faults : new Units(value);
    }

    public static OneOf<Maybe<Units>, IEnumerable<DomainRuleFault>> CreateOptional(int? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Maybe<Units>, IEnumerable<DomainRuleFault>> CreateOptional(int? value, int minValue, int maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<Units>, IEnumerable<DomainRuleFault>>(Maybe<Units>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<Units>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Units>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Units Convert(int value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new Units(int.MinValue)
        );
    
    public int CompareTo(Units? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}