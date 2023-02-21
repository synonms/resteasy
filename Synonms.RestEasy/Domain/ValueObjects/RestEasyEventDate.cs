using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Domain.ValueObjects;

public record RestEasyEventDate : ValueObject<DateOnly>, IComparable, IComparable<RestEasyEventDate>
{
    private RestEasyEventDate(DateOnly value) : base(value)
    {
    }
        
    public static implicit operator DateOnly(RestEasyEventDate valueObject) => valueObject.Value;

    public static OneOf<RestEasyEventDate, IEnumerable<DomainRuleFault>> CreateMandatory(DateOnly value) =>
        CreateMandatory(value, DateOnly.MinValue, DateOnly.MaxValue);

    public static OneOf<RestEasyEventDate, IEnumerable<DomainRuleFault>> CreateMandatory(DateOnly value, DateOnly minimum, DateOnly maximum)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minimum)
        {
            faults.Add(new DomainRuleFault("{0} property has minimum of {1}.", nameof(RestEasyEventDate), minimum));
        }

        if (value > maximum)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum of {1}.", nameof(RestEasyEventDate), maximum));
        }
            
        return faults.Any() ? faults : new RestEasyEventDate(value);
    }

    public static OneOf<Maybe<RestEasyEventDate>, IEnumerable<DomainRuleFault>> CreateOptional(DateOnly? value, int maxLength) =>
        CreateOptional(value, DateOnly.MinValue, DateOnly.MaxValue);

    public static OneOf<Maybe<RestEasyEventDate>, IEnumerable<DomainRuleFault>> CreateOptional(DateOnly? value, DateOnly minimum, DateOnly maximum)
    {
        if (value is null)
        {
            return new OneOf<Maybe<RestEasyEventDate>, IEnumerable<DomainRuleFault>>(Maybe<RestEasyEventDate>.None);
        }

        return CreateMandatory(value.Value, minimum, maximum).Match(
            valueObject => new OneOf<Maybe<RestEasyEventDate>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<RestEasyEventDate>, IEnumerable<DomainRuleFault>>(faults));
    }
        
    public static RestEasyEventDate Convert(DateOnly value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new RestEasyEventDate(DateOnly.MinValue)
        );
        
    public int CompareTo(RestEasyEventDate? other) => DateTime.Compare(Value.ToDateTime(TimeOnly.MinValue), other?.Value.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue);
        
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}