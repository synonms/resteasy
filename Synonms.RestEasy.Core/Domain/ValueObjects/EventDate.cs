using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record EventDate : ValueObject<DateOnly>, IComparable, IComparable<EventDate>
{
    private EventDate(DateOnly value) : base(value)
    {
    }

    public static implicit operator DateOnly(EventDate valueObject) => valueObject.Value;

    public static OneOf<EventDate, IEnumerable<DomainRuleFault>> CreateMandatory(DateOnly value) =>
        CreateMandatory(value, DateOnly.MinValue, DateOnly.MaxValue);

    public static OneOf<EventDate, IEnumerable<DomainRuleFault>> CreateMandatory(DateOnly value, DateOnly minimum, DateOnly maximum)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minimum)
        {
            faults.Add(new DomainRuleFault("{0} property has minimum of {1}.", nameof(EventDate), minimum));
        }

        if (value > maximum)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum of {1}.", nameof(EventDate), maximum));
        }
            
        return faults.Any() ? faults : new EventDate(value);
    }

    public static OneOf<Maybe<EventDate>, IEnumerable<DomainRuleFault>> CreateOptional(DateOnly? value) =>
        CreateOptional(value, DateOnly.MinValue, DateOnly.MaxValue);

    public static OneOf<Maybe<EventDate>, IEnumerable<DomainRuleFault>> CreateOptional(DateOnly? value, DateOnly minimum, DateOnly maximum)
    {
        if (value is null)
        {
            return new OneOf<Maybe<EventDate>, IEnumerable<DomainRuleFault>>(Maybe<EventDate>.None);
        }

        return CreateMandatory(value.Value, minimum, maximum).Match(
            valueObject => new OneOf<Maybe<EventDate>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<EventDate>, IEnumerable<DomainRuleFault>>(faults));
    }
        
    internal static EventDate Convert(DateOnly value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new EventDate(DateOnly.MinValue)
        );
        
    public int CompareTo(EventDate? other) => DateTime.Compare(Value.ToDateTime(TimeOnly.MinValue), other?.Value.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue);
        
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}