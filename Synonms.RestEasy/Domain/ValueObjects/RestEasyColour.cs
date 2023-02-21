using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Domain.ValueObjects;

public record RestEasyColour : ValueObject<string>, IComparable, IComparable<RestEasyColour>
{
    private RestEasyColour(string value) : base(value)
    {
    }
    
    public static implicit operator string(RestEasyColour valueObject) => valueObject.Value;

    public static OneOf<RestEasyColour, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(RestEasyColour)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(RestEasyColour), maxLength));
        }
        
        return faults.Any() ? faults : new RestEasyColour(value);
    }
    
    public static OneOf<Maybe<RestEasyColour>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<RestEasyColour>, IEnumerable<DomainRuleFault>>(Maybe<RestEasyColour>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<RestEasyColour>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<RestEasyColour>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    public static RestEasyColour Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new RestEasyColour(string.Empty)
        );
    
    public int CompareTo(RestEasyColour? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}