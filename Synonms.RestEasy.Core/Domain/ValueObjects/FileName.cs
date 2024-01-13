using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record FileName : ValueObject<string>, IComparable, IComparable<FileName>
{
    private FileName(string value) : base(value)
    {
    }
    
    public static implicit operator string(FileName valueObject) => valueObject.Value;

    public static OneOf<FileName, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(FileName)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(FileName), maxLength));
        }
        
        return faults.Any() ? faults : new FileName(value);
    }
    
    public static OneOf<Maybe<FileName>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<FileName>, IEnumerable<DomainRuleFault>>(Maybe<FileName>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<FileName>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<FileName>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static FileName Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new FileName(string.Empty)
        );
    
    public int CompareTo(FileName? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}