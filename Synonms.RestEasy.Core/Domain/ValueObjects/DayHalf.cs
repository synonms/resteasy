using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record DayHalf : ValueObject<string>, IComparable, IComparable<DayHalf>
{
    public static readonly string[] AcceptableValues = 
    {
        "AM",
        "PM"
    };
    
    private DayHalf(string value) : base(value)
    {
    }
    
    public static implicit operator string(DayHalf valueObject) => valueObject.Value;

    public static DayHalf Am => new("AM");
    public static DayHalf Pm => new("PM");
    
    public static OneOf<DayHalf, IEnumerable<DomainRuleFault>> CreateMandatory(string value)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(DayHalf)));
            return faults;
        }

        if (AcceptableValues.Contains(value) is false)
        {
            faults.Add(new DomainRuleFault("{0} property has value '{1}' which is not currently in the list of available values.", nameof(DayHalf), value));
        }
        
        return faults.Any() ? faults : new DayHalf(value);
    }

    public static OneOf<Maybe<DayHalf>, IEnumerable<DomainRuleFault>> CreateOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<DayHalf>, IEnumerable<DomainRuleFault>>(Maybe<DayHalf>.None);
        }

        return CreateMandatory(value).Match(
            valueObject => new OneOf<Maybe<DayHalf>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<DayHalf>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static DayHalf Convert(string value) =>
        CreateMandatory(value is not "AM" or "PM" ? "AM" : value).Match(
            valueObject => valueObject,
            fault => new DayHalf(string.Empty)
        );
    
    public int CompareTo(DayHalf? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}