using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Year : ValueObject<int>, IComparable, IComparable<Year>
{
    private const int DefaultMinValue = 1970;
    private const int DefaultMaxValue = 2070;
    
    private Year(int value) : base(value)
    {
    }
    
    public static implicit operator int(Year valueObject) => valueObject.Value;

    public static OneOf<Year, IEnumerable<DomainRuleFault>> CreateMandatory(int value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Year, IEnumerable<DomainRuleFault>> CreateMandatory(int value, int minValue, int maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(Year), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(Year), maxValue));
        }

        return faults.Any() ? faults : new Year(value);
    }

    public static OneOf<Maybe<Year>, IEnumerable<DomainRuleFault>> CreateOptional(int? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Maybe<Year>, IEnumerable<DomainRuleFault>> CreateOptional(int? value, int minValue, int maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<Year>, IEnumerable<DomainRuleFault>>(Maybe<Year>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<Year>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Year>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Year Convert(int value) =>
        CreateMandatory(value, int.MinValue, int.MaxValue).Match(
            valueObject => valueObject,
            fault => new Year(int.MinValue)
        );
    
    public int CompareTo(Year? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}