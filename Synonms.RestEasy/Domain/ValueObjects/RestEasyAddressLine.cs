using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Domain.ValueObjects;

public record RestEasyAddressLine : ValueObject<string>, IComparable, IComparable<RestEasyAddressLine>
{
    private const int DefaultMaxLength = 40;
    
    private RestEasyAddressLine(string value) : base(value)
    {
    }
    
    public static implicit operator string(RestEasyAddressLine valueObject) => valueObject.Value;

    public static OneOf<RestEasyAddressLine, IEnumerable<DomainRuleFault>> CreateMandatory(string value) =>
        CreateMandatory(value, DefaultMaxLength);

    public static OneOf<RestEasyAddressLine, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(RestEasyAddressLine)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(RestEasyAddressLine), maxLength));
        }
        
        return faults.Any() ? faults : new RestEasyAddressLine(value);
    }

    public static OneOf<Maybe<RestEasyAddressLine>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) =>
        CreateOptional(value, DefaultMaxLength);

    public static OneOf<Maybe<RestEasyAddressLine>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<RestEasyAddressLine>, IEnumerable<DomainRuleFault>>(Maybe<RestEasyAddressLine>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<RestEasyAddressLine>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<RestEasyAddressLine>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    public static RestEasyAddressLine Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new RestEasyAddressLine(string.Empty)
        );
    
    public int CompareTo(RestEasyAddressLine? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}