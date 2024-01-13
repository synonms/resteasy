using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Weeks : ValueObject<int>, IComparable, IComparable<Weeks>
{
    private const int DefaultMinValue = 0;
    private const int DefaultMaxValue = int.MaxValue;

    private Weeks(int value) : base(value)
    {
    }
    
    public static implicit operator int(Weeks valueObject) => valueObject.Value;

    public static OneOf<Weeks, IEnumerable<DomainRuleFault>> CreateMandatory(int value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Weeks, IEnumerable<DomainRuleFault>> CreateMandatory(int value, int minValue, int maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(Weeks), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(Weeks), maxValue));
        }

        return faults.Any() ? faults : new Weeks(value);
    }

    public static OneOf<Maybe<Weeks>, IEnumerable<DomainRuleFault>> CreateOptional(int? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Maybe<Weeks>, IEnumerable<DomainRuleFault>> CreateOptional(int? value, int minValue, int maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<Weeks>, IEnumerable<DomainRuleFault>>(Maybe<Weeks>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<Weeks>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Weeks>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Weeks Convert(int value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new Weeks(int.MinValue)
        );
    
    public int CompareTo(Weeks? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}