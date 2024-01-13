using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Money : ValueObject<decimal>, IComparable, IComparable<Money>
{
    private Money(decimal value) : base(value)
    {
    }
        
    public static implicit operator decimal(Money valueObject) => valueObject.Value;

    public static OneOf<Money, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value) =>
        CreateMandatory(value, decimal.MinValue, decimal.MaxValue);

    public static OneOf<Money, IEnumerable<DomainRuleFault>> CreateMandatory(decimal value, decimal minimum, decimal maximum)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minimum)
        {
            faults.Add(new DomainRuleFault("{0} property has minimum of {1}.", nameof(Money), minimum));
        }

        if (value > maximum)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum of {1}.", nameof(Money), maximum));
        }
            
        return faults.Any() ? faults : new Money(value);
    }

    public static OneOf<Maybe<Money>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value) =>
        CreateOptional(value, decimal.MinValue, decimal.MaxValue);

    public static OneOf<Maybe<Money>, IEnumerable<DomainRuleFault>> CreateOptional(decimal? value, decimal minimum, decimal maximum)
    {
        if (value is null)
        {
            return new OneOf<Maybe<Money>, IEnumerable<DomainRuleFault>>(Maybe<Money>.None);
        }

        return CreateMandatory(value.Value, minimum, maximum).Match(
            valueObject => new OneOf<Maybe<Money>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Money>, IEnumerable<DomainRuleFault>>(faults));
    }
        
    internal static Money Convert(decimal value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new Money(decimal.MinValue)
        );
        
    public int CompareTo(Money? other) => decimal.Compare(Value, other?.Value ?? decimal.MinValue);
        
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}