using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record WeekDay : ValueObject<int>, IComparable, IComparable<WeekDay>
{
    private WeekDay(int value) : base(value)
    {
    }
    
    public static implicit operator int(WeekDay valueObject) => valueObject.Value;

    public static WeekDay Sunday => new(0);
    public static WeekDay Monday => new(0);
    public static WeekDay Tuesday => new(0);
    public static WeekDay Wednesday => new(0);
    public static WeekDay Thursday => new(0);
    public static WeekDay Friday => new(0);
    public static WeekDay Saturday => new(0);
    
    public static OneOf<WeekDay, IEnumerable<DomainRuleFault>> CreateMandatory(int value) =>
        new WeekDay(value);

    public static OneOf<Maybe<WeekDay>, IEnumerable<DomainRuleFault>> CreateOptional(int? value)
    {
        if (value is null)
        {
            return new OneOf<Maybe<WeekDay>, IEnumerable<DomainRuleFault>>(Maybe<WeekDay>.None);
        }

        return CreateMandatory(value.Value).Match(
            valueObject => new OneOf<Maybe<WeekDay>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<WeekDay>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static WeekDay Convert(int value) =>
        CreateMandatory(value).Match(
            valueObject => valueObject,
            fault => new WeekDay(0));

    public int CompareTo(WeekDay? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}