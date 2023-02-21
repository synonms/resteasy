using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Domain.ValueObjects;

public record RestEasyMoniker : ValueObject<string>, IComparable, IComparable<RestEasyMoniker>
{
    private RestEasyMoniker(string value) : base(value)
    {
    }
    
    public static implicit operator string(RestEasyMoniker valueObject) => valueObject.Value;

    public static OneOf<RestEasyMoniker, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(RestEasyMoniker)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(RestEasyMoniker), maxLength));
        }
        
        return faults.Any() ? faults : new RestEasyMoniker(value);
    }
    
    public static OneOf<Maybe<RestEasyMoniker>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<RestEasyMoniker>, IEnumerable<DomainRuleFault>>(Maybe<RestEasyMoniker>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<RestEasyMoniker>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<RestEasyMoniker>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    public static RestEasyMoniker Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new RestEasyMoniker(string.Empty)
        );
    
    public int CompareTo(RestEasyMoniker? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}