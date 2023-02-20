using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Domain.ValueObjects;

public record Description : ValueObject<string>, IComparable, IComparable<Description>
{
    private Description(string value) : base(value)
    {
    }
    
    public static implicit operator string(Description valueObject) => valueObject.Value;

    public static OneOf<Description, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(Description)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(Description), maxLength));
        }
        
        return faults.Any() ? faults : new Description(value);
    }
    
    public static OneOf<Maybe<Description>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<Description>, IEnumerable<DomainRuleFault>>(Maybe<Description>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<Description>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Description>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Description Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new Description(string.Empty)
        );
    
    public int CompareTo(Description? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}