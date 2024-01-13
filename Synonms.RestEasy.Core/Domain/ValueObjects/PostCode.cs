using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record PostCode : ValueObject<string>, IComparable, IComparable<PostCode>
{
    private const int DefaultMaxLength = 10;
    
    private PostCode(string value) : base(value)
    {
    }
    
    public static implicit operator string(PostCode valueObject) => valueObject.Value;

    public static OneOf<PostCode, IEnumerable<DomainRuleFault>> CreateMandatory(string value) =>
        CreateMandatory(value, DefaultMaxLength);

    public static OneOf<PostCode, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(PostCode)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(PostCode), maxLength));
        }
        
        return faults.Any() ? faults : new PostCode(value);
    }

    public static OneOf<Maybe<PostCode>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) =>
        CreateOptional(value, DefaultMaxLength);

    public static OneOf<Maybe<PostCode>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<PostCode>, IEnumerable<DomainRuleFault>>(Maybe<PostCode>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<PostCode>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<PostCode>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static PostCode Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new PostCode(string.Empty)
        );
    
    public int CompareTo(PostCode? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}