using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Reference : ValueObject<string>, IComparable, IComparable<Reference>
{
    public const int DefaultMaxLength = 10;
    
    private Reference(string value) : base(value)
    {
    }
    
    public static implicit operator string(Reference valueObject) => valueObject.Value;

    public static OneOf<Reference, IEnumerable<DomainRuleFault>> CreateMandatory(string value) =>
        CreateMandatory(value, DefaultMaxLength);

    public static OneOf<Reference, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(Reference)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(Reference), maxLength));
        }
        
        return faults.Any() ? faults : new Reference(value);
    }

    public static OneOf<Maybe<Reference>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) =>
        CreateOptional(value, DefaultMaxLength);

    public static OneOf<Maybe<Reference>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<Reference>, IEnumerable<DomainRuleFault>>(Maybe<Reference>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<Reference>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Reference>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Reference Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new Reference(string.Empty)
        );
    
    public int CompareTo(Reference? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}