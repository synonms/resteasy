using System.Text.RegularExpressions;
using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;
using Synonms.RestEasy.Core.Text;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record TelephoneNumber : ValueObject<string>, IComparable, IComparable<TelephoneNumber>
{
    public const int MaxLength = 12;
    
    private TelephoneNumber(string value) : base(value)
    {
    }
    
    public static implicit operator string(TelephoneNumber valueObject) => valueObject.Value;

    public static OneOf<TelephoneNumber, IEnumerable<DomainRuleFault>> CreateMandatory(string value)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(TelephoneNumber)));
            return faults;
        }

        if (value.Length > MaxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(TelephoneNumber), MaxLength));
        }
        
        if (Regex.IsMatch(value, RegularExpressions.TelephoneNumber) is false)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.PatternError, nameof(TelephoneNumber), RegularExpressions.TelephoneNumber));
        }
        
        return faults.Any() ? faults : new TelephoneNumber(value);
    }
    
    public static OneOf<Maybe<TelephoneNumber>, IEnumerable<DomainRuleFault>> CreateOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<TelephoneNumber>, IEnumerable<DomainRuleFault>>(Maybe<TelephoneNumber>.None);
        }

        return CreateMandatory(value).Match(
            valueObject => new OneOf<Maybe<TelephoneNumber>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<TelephoneNumber>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static TelephoneNumber Convert(string value) =>
        CreateMandatory(value).Match(
            valueObject => valueObject,
            fault => new TelephoneNumber(string.Empty)
        );
    
    public int CompareTo(TelephoneNumber? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}