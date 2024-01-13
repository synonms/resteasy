using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Initials : ValueObject<string>, IComparable, IComparable<Initials>
{
    public const int DefaultMaxLength = 5;
    
    private Initials(string value) : base(value)
    {
    }
    
    public static implicit operator string(Initials valueObject) => valueObject.Value;

    public static OneOf<Initials, IEnumerable<DomainRuleFault>> CreateMandatory(string value) =>
        CreateMandatory(value, DefaultMaxLength);

    public static OneOf<Initials, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(Initials)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(Initials), maxLength));
        }
        
        return faults.Any() ? faults : new Initials(value);
    }

    public static OneOf<Maybe<Initials>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) =>
        CreateOptional(value, DefaultMaxLength);

    public static OneOf<Maybe<Initials>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<Initials>, IEnumerable<DomainRuleFault>>(Maybe<Initials>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<Initials>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Initials>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Initials Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new Initials(string.Empty)
        );
    
    public int CompareTo(Initials? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}