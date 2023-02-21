using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Domain.ValueObjects;

public record RestEasyDescription : ValueObject<string>, IComparable, IComparable<RestEasyDescription>
{
    private RestEasyDescription(string value) : base(value)
    {
    }
    
    public static implicit operator string(RestEasyDescription valueObject) => valueObject.Value;

    public static OneOf<RestEasyDescription, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(RestEasyDescription)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(RestEasyDescription), maxLength));
        }
        
        return faults.Any() ? faults : new RestEasyDescription(value);
    }
    
    public static OneOf<Maybe<RestEasyDescription>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<RestEasyDescription>, IEnumerable<DomainRuleFault>>(Maybe<RestEasyDescription>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<RestEasyDescription>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<RestEasyDescription>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    public static RestEasyDescription Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new RestEasyDescription(string.Empty)
        );
    
    public int CompareTo(RestEasyDescription? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}