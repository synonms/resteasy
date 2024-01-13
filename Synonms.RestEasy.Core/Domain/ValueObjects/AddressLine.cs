using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record AddressLine : ValueObject<string>, IComparable, IComparable<AddressLine>
{
    private const int DefaultMaxLength = 40;
    
    private AddressLine(string value) : base(value)
    {
    }
    
    public static implicit operator string(AddressLine valueObject) => valueObject.Value;

    public static OneOf<AddressLine, IEnumerable<DomainRuleFault>> CreateMandatory(string value) =>
        CreateMandatory(value, DefaultMaxLength);

    public static OneOf<AddressLine, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(AddressLine)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(AddressLine), maxLength));
        }
        
        return faults.Any() ? faults : new AddressLine(value);
    }

    public static OneOf<Maybe<AddressLine>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) =>
        CreateOptional(value, DefaultMaxLength);

    public static OneOf<Maybe<AddressLine>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<AddressLine>, IEnumerable<DomainRuleFault>>(Maybe<AddressLine>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<AddressLine>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<AddressLine>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static AddressLine Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new AddressLine(string.Empty)
        );
    
    public int CompareTo(AddressLine? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}