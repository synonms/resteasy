using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Domain.ValueObjects;

public record Colour : ValueObject<string>, IComparable, IComparable<Colour>
{
    private Colour(string value) : base(value)
    {
    }
    
    public static implicit operator string(Colour valueObject) => valueObject.Value;

    public static OneOf<Colour, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(Colour)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(Colour), maxLength));
        }
        
        return faults.Any() ? faults : new Colour(value);
    }
    
    public static OneOf<Maybe<Colour>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<Colour>, IEnumerable<DomainRuleFault>>(Maybe<Colour>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<Colour>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Colour>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Colour Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new Colour(string.Empty)
        );
    
    public int CompareTo(Colour? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}