using System.Text.RegularExpressions;
using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record BankSortCode : ValueObject<string>, IComparable, IComparable<BankSortCode>
{
    public const string Pattern = @"^(\d){6}$";

    private BankSortCode(string value) : base(value)
    {
    }
    
    public static implicit operator string(BankSortCode valueObject) => valueObject.Value;
    
    public static OneOf<BankSortCode, IEnumerable<DomainRuleFault>> CreateMandatory(string value)
    {
        List<DomainRuleFault> faults = new ();
        
        if (Regex.IsMatch(value, Pattern) is false)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.PatternError, nameof(BankSortCode), Pattern));
        }
        
        return faults.Any() ? faults : new BankSortCode(value);
    }
    
    public static OneOf<Maybe<BankSortCode>, IEnumerable<DomainRuleFault>> CreateOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<BankSortCode>, IEnumerable<DomainRuleFault>>(Maybe<BankSortCode>.None);
        }

        return CreateMandatory(value).Match(
            valueObject => new OneOf<Maybe<BankSortCode>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<BankSortCode>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static BankSortCode Convert(string value) =>
        CreateMandatory(value).Match(
            valueObject => valueObject,
            fault => new BankSortCode(string.Empty));
    
    public int CompareTo(BankSortCode? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}