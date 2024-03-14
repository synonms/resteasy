using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record SexAssignedAtBirth : ValueObject<string>, IComparable, IComparable<SexAssignedAtBirth>
{
    public static readonly string[] DefaultAcceptableValues = 
    {
        "Unspecified",
        "Female",
        "Male",
        "Intersex"
    };
    
    private SexAssignedAtBirth(string value) : base(value)
    {
    }
    
    public static implicit operator string(SexAssignedAtBirth valueObject) => valueObject.Value;

    public static SexAssignedAtBirth Unspecified => new("Unspecified");
    public static SexAssignedAtBirth Female => new("Female");
    public static SexAssignedAtBirth Male => new("Male");
    public static SexAssignedAtBirth Intersex => new("Intersex");
    
    public static OneOf<SexAssignedAtBirth, IEnumerable<DomainRuleFault>> CreateMandatory(string value) =>
        CreateMandatory(value, DefaultAcceptableValues);

    public static OneOf<SexAssignedAtBirth, IEnumerable<DomainRuleFault>> CreateMandatory(string value, IEnumerable<string> acceptableValues)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(SexAssignedAtBirth)));
            return faults;
        }

        if (acceptableValues.Contains(value) is false)
        {
            faults.Add(new DomainRuleFault("{0} property has value '{1}' which is not currently in the list of available values.", nameof(SexAssignedAtBirth), value));
        }
        
        return faults.Any() ? faults : new SexAssignedAtBirth(value);
    }

    public static OneOf<Maybe<SexAssignedAtBirth>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) =>
        CreateOptional(value, DefaultAcceptableValues);

    public static OneOf<Maybe<SexAssignedAtBirth>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, IEnumerable<string> acceptableValues)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<SexAssignedAtBirth>, IEnumerable<DomainRuleFault>>(Maybe<SexAssignedAtBirth>.None);
        }

        return CreateMandatory(value, acceptableValues).Match(
            valueObject => new OneOf<Maybe<SexAssignedAtBirth>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<SexAssignedAtBirth>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static SexAssignedAtBirth Convert(string value) =>
        CreateMandatory(value, new[] { value }).Match(
            valueObject => valueObject,
            fault => new SexAssignedAtBirth(string.Empty)
        );
    
    public int CompareTo(SexAssignedAtBirth? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}