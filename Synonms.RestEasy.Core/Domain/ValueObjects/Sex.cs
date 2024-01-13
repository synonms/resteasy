using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Sex : ValueObject<string>, IComparable, IComparable<Sex>
{
    public static readonly string[] DefaultAcceptableValues = 
    {
        "Male",
        "Female"
    };
    
    private Sex(string value) : base(value)
    {
    }
    
    public static implicit operator string(Sex valueObject) => valueObject.Value;

    public static OneOf<Sex, IEnumerable<DomainRuleFault>> CreateMandatory(string value) =>
        CreateMandatory(value, DefaultAcceptableValues);

    public static OneOf<Sex, IEnumerable<DomainRuleFault>> CreateMandatory(string value, IEnumerable<string> acceptableValues)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(Sex)));
            return faults;
        }

        if (acceptableValues.Contains(value) is false)
        {
            faults.Add(new DomainRuleFault("{0} property has value '{1}' which is not currently in the list of available values.", nameof(Sex), value));
        }
        
        return faults.Any() ? faults : new Sex(value);
    }

    public static OneOf<Maybe<Sex>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) =>
        CreateOptional(value, DefaultAcceptableValues);

    public static OneOf<Maybe<Sex>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, IEnumerable<string> acceptableValues)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<Sex>, IEnumerable<DomainRuleFault>>(Maybe<Sex>.None);
        }

        return CreateMandatory(value, acceptableValues).Match(
            valueObject => new OneOf<Maybe<Sex>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Sex>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Sex Convert(string value) =>
        CreateMandatory(value, new[] { value }).Match(
            valueObject => valueObject,
            fault => new Sex(string.Empty)
        );
    
    public int CompareTo(Sex? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}