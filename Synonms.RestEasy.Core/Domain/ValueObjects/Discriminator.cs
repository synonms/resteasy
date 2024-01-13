using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Discriminator : ValueObject<string>, IComparable, IComparable<Discriminator>
{
    private Discriminator(string value) : base(value)
    {
    }
    
    public static implicit operator string(Discriminator valueObject) => valueObject.Value;

    public static OneOf<Discriminator, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(Discriminator)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(Discriminator), maxLength));
        }
        
        return faults.Any() ? faults : new Discriminator(value);
    }
    
    public static OneOf<Maybe<Discriminator>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<Discriminator>, IEnumerable<DomainRuleFault>>(Maybe<Discriminator>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<Discriminator>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Discriminator>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Discriminator Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new Discriminator(string.Empty)
        );
    
    public int CompareTo(Discriminator? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}