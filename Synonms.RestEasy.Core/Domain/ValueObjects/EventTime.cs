using Synonms.RestEasy.Core.Extensions;
using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record EventTime : ValueObject<TimeOnly>, IComparable, IComparable<EventTime>
{
    private EventTime(TimeOnly value) : base(value)
    {
    }

    public static implicit operator TimeOnly(EventTime valueObject) => valueObject.Value;

    public static OneOf<EventTime, IEnumerable<DomainRuleFault>> CreateMandatory(TimeOnly value) =>
        CreateMandatory(value, TimeOnly.MinValue, TimeOnly.MaxValue);

    public static OneOf<EventTime, IEnumerable<DomainRuleFault>> CreateMandatory(TimeOnly value, TimeOnly minimum, TimeOnly maximum)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minimum)
        {
            faults.Add(new DomainRuleFault("{0} property has minimum of {1}.", nameof(EventTime), minimum));
        }

        if (value > maximum)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum of {1}.", nameof(EventTime), maximum));
        }
            
        return faults.Any() ? faults : new EventTime(value);
    }

    public static OneOf<Maybe<EventTime>, IEnumerable<DomainRuleFault>> CreateOptional(TimeOnly? value) =>
        CreateOptional(value, TimeOnly.MinValue, TimeOnly.MaxValue);

    public static OneOf<Maybe<EventTime>, IEnumerable<DomainRuleFault>> CreateOptional(TimeOnly? value, TimeOnly minimum, TimeOnly maximum)
    {
        if (value is null)
        {
            return new OneOf<Maybe<EventTime>, IEnumerable<DomainRuleFault>>(Maybe<EventTime>.None);
        }

        return CreateMandatory(value.Value, minimum, maximum).Match(
            valueObject => new OneOf<Maybe<EventTime>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<EventTime>, IEnumerable<DomainRuleFault>>(faults));
    }
        
    internal static EventTime Convert(TimeOnly value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new EventTime(TimeOnly.MinValue)
        );
        
    public int CompareTo(EventTime? other) => DateTime.Compare(Value.ToDateTime(DateOnly.MinValue), other?.Value.ToDateTime(DateOnly.MinValue) ?? DateTime.MinValue);
        
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}