using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record GeneralLedgerCode : ValueObject<string>, IComparable, IComparable<GeneralLedgerCode>
{
    private GeneralLedgerCode(string value) : base(value)
    {
    }
    
    public static implicit operator string(GeneralLedgerCode valueObject) => valueObject.Value;
    
    public static OneOf<GeneralLedgerCode, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(GeneralLedgerCode)));
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(GeneralLedgerCode), maxLength));
        }
        
        return faults.Any() ? faults : new GeneralLedgerCode(value);
    }
    
    public static OneOf<Maybe<GeneralLedgerCode>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<GeneralLedgerCode>, IEnumerable<DomainRuleFault>>(Maybe<GeneralLedgerCode>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<GeneralLedgerCode>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<GeneralLedgerCode>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static GeneralLedgerCode Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new GeneralLedgerCode(string.Empty)
        );
    
    public int CompareTo(GeneralLedgerCode? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}