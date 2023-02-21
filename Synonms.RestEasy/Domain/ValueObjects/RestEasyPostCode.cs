using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Domain.ValueObjects;

public record RestEasyPostCode : ValueObject<string>, IComparable, IComparable<RestEasyPostCode>
{
    private const int DefaultMaxLength = 10;
    
    private RestEasyPostCode(string value) : base(value)
    {
    }
    
    public static implicit operator string(RestEasyPostCode valueObject) => valueObject.Value;

    public static OneOf<RestEasyPostCode, IEnumerable<DomainRuleFault>> CreateMandatory(string value) =>
        CreateMandatory(value, DefaultMaxLength);

    public static OneOf<RestEasyPostCode, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(RestEasyPostCode)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(RestEasyPostCode), maxLength));
        }
        
        return faults.Any() ? faults : new RestEasyPostCode(value);
    }

    public static OneOf<Maybe<RestEasyPostCode>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) =>
        CreateOptional(value, DefaultMaxLength);

    public static OneOf<Maybe<RestEasyPostCode>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<RestEasyPostCode>, IEnumerable<DomainRuleFault>>(Maybe<RestEasyPostCode>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<RestEasyPostCode>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<RestEasyPostCode>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    public static RestEasyPostCode Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new RestEasyPostCode(string.Empty)
        );
    
    public int CompareTo(RestEasyPostCode? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}