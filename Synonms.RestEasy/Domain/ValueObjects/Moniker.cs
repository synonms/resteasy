using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Domain.ValueObjects;

public record Moniker : ValueObject<string>, IComparable, IComparable<Moniker>
{
    private Moniker(string value) : base(value)
    {
    }
    
    public static implicit operator string(Moniker valueObject) => valueObject.Value;

    public static OneOf<Moniker, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(Moniker)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(Moniker), maxLength));
        }
        
        return faults.Any() ? faults : new Moniker(value);
    }
    
    public static OneOf<Maybe<Moniker>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<Moniker>, IEnumerable<DomainRuleFault>>(Maybe<Moniker>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<Moniker>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Moniker>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Moniker Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new Moniker(string.Empty)
        );
    
    public int CompareTo(Moniker? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}