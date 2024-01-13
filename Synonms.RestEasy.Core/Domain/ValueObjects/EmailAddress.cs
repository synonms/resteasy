using System.Text.RegularExpressions;
using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;
using Synonms.RestEasy.Core.Text;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record EmailAddress : ValueObject<string>, IComparable, IComparable<EmailAddress>
{
    public const int MaxLength = 255;

    private EmailAddress(string value) : base(value)
    {
    }
    
    public static implicit operator string(EmailAddress valueObject) => valueObject.Value;

    public static OneOf<EmailAddress, IEnumerable<DomainRuleFault>> CreateMandatory(string value)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(EmailAddress)));
            return faults;
        }

        if (value.Length > MaxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(EmailAddress), MaxLength));
        }

        if (Regex.IsMatch(value, RegularExpressions.Email) is false)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.PatternError, nameof(EmailAddress), RegularExpressions.Email));
        }

        return faults.Any() ? faults : new EmailAddress(value);
    }
    
    public static OneOf<Maybe<EmailAddress>, IEnumerable<DomainRuleFault>> CreateOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<EmailAddress>, IEnumerable<DomainRuleFault>>(Maybe<EmailAddress>.None);
        }

        return CreateMandatory(value).Match(
            valueObject => new OneOf<Maybe<EmailAddress>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<EmailAddress>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static EmailAddress Convert(string value) =>
        CreateMandatory(value).Match(
            valueObject => valueObject,
            fault => new EmailAddress(string.Empty)
        );
    
    public int CompareTo(EmailAddress? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}