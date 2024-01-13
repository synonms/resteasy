using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record EmploymentType : ValueObject<string>, IComparable, IComparable<EmploymentType>
{
    public static readonly string[] DefaultAcceptableValues = 
    {
        "Permanent",
        "Contract",
        "Zero Hours"
    };
    
    private EmploymentType(string value) : base(value)
    {
    }
    
    public static implicit operator string(EmploymentType valueObject) => valueObject.Value;
    
    public static OneOf<EmploymentType, IEnumerable<DomainRuleFault>> CreateMandatory(string value) =>
        CreateMandatory(value, DefaultAcceptableValues);

    public static OneOf<EmploymentType, IEnumerable<DomainRuleFault>> CreateMandatory(string value, IEnumerable<string> acceptableValues)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(EmploymentType)));
            return faults;
        }

        if (acceptableValues.Contains(value) is false)
        {
            faults.Add(new DomainRuleFault("{0} property has value '{1}' which is not currently in the list of available values.", nameof(EmploymentType), value));
        }
        
        return faults.Any() ? faults : new EmploymentType(value);
    }

    public static OneOf<Maybe<EmploymentType>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) =>
        CreateOptional(value, DefaultAcceptableValues);

    public static OneOf<Maybe<EmploymentType>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, IEnumerable<string> acceptableValues)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<EmploymentType>, IEnumerable<DomainRuleFault>>(Maybe<EmploymentType>.None);
        }

        return CreateMandatory(value, acceptableValues).Match(
            valueObject => new OneOf<Maybe<EmploymentType>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<EmploymentType>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static EmploymentType Convert(string value) =>
        CreateMandatory(value, new[] { value }).Match(
            valueObject => valueObject,
            fault => new EmploymentType(string.Empty)
        );
    
    public int CompareTo(EmploymentType? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}