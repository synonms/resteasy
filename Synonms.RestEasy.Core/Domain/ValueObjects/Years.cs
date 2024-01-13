using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Years : ValueObject<int>, IComparable, IComparable<Years>
{
    private const int DefaultMinValue = 0;
    private const int DefaultMaxValue = int.MaxValue;

    private Years(int value) : base(value)
    {
    }
    
    public static implicit operator int(Years valueObject) => valueObject.Value;

    public static OneOf<Years, IEnumerable<DomainRuleFault>> CreateMandatory(int value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Years, IEnumerable<DomainRuleFault>> CreateMandatory(int value, int minValue, int maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(Years), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(Years), maxValue));
        }

        return faults.Any() ? faults : new Years(value);
    }

    public static OneOf<Maybe<Years>, IEnumerable<DomainRuleFault>> CreateOptional(int? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Maybe<Years>, IEnumerable<DomainRuleFault>> CreateOptional(int? value, int minValue, int maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<Years>, IEnumerable<DomainRuleFault>>(Maybe<Years>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<Years>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Years>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Years Convert(int value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new Years(int.MinValue)
        );
    
    public int CompareTo(Years? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}