using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Code : ValueObject<string>, IComparable, IComparable<Code>
{
    public const int DefaultMaxLength = 10;
    
    private Code(string value) : base(value)
    {
    }
    
    public static implicit operator string(Code valueObject) => valueObject.Value;

    public static OneOf<Code, IEnumerable<DomainRuleFault>> CreateMandatory(string value) =>
        CreateMandatory(value, DefaultMaxLength);

    public static OneOf<Code, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(Code)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(Code), maxLength));
        }
        
        return faults.Any() ? faults : new Code(value);
    }

    public static OneOf<Maybe<Code>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) =>
        CreateOptional(value, DefaultMaxLength);

    public static OneOf<Maybe<Code>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<Code>, IEnumerable<DomainRuleFault>>(Maybe<Code>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<Code>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Code>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Code Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new Code(string.Empty)
        );
    
    public int CompareTo(Code? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}