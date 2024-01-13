using System.Text.RegularExpressions;
using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record BankAccountNumber : ValueObject<string>, IComparable, IComparable<BankAccountNumber>
{
    public const string Pattern = @"^(\d){6,8}$";

    private BankAccountNumber(string value) : base(value)
    {
    }
    
    public static implicit operator string(BankAccountNumber valueObject) => valueObject.Value;

    public static OneOf<BankAccountNumber, IEnumerable<DomainRuleFault>> CreateMandatory(string value)
    {
        List<DomainRuleFault> faults = new ();

        if (Regex.IsMatch(value, Pattern) is false)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.PatternError, nameof(BankAccountNumber), Pattern));
        }

        return faults.Any() ? faults : new BankAccountNumber(value);
    }
    
    public static OneOf<Maybe<BankAccountNumber>, IEnumerable<DomainRuleFault>> CreateOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<BankAccountNumber>, IEnumerable<DomainRuleFault>>(Maybe<BankAccountNumber>.None);
        }

        return CreateMandatory(value).Match(
            valueObject => new OneOf<Maybe<BankAccountNumber>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<BankAccountNumber>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static BankAccountNumber Convert(string value) =>
        CreateMandatory(value).Match(
            valueObject => valueObject,
            fault => new BankAccountNumber(string.Empty));
    
    public int CompareTo(BankAccountNumber? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}