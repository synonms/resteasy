using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record PaymentFrequency : ValueObject<string>, IComparable, IComparable<PaymentFrequency>
{
    public static readonly string[] DefaultAcceptableValues = 
    {
        "Weekly",
        "Fortnightly",
        "Every 4 weeks",
        "Monthly",
        "Quarterly",
        "Twice a year",
        "Annually",
        "One-off payment",
        "Irregular"
    };

    public static PaymentFrequency Monthly => new("Monthly");
    
    private PaymentFrequency(string value) : base(value)
    {
    }
    
    public static implicit operator string(PaymentFrequency valueObject) => valueObject.Value;

    public static OneOf<PaymentFrequency, IEnumerable<DomainRuleFault>> CreateMandatory(string value) =>
        CreateMandatory(value, DefaultAcceptableValues);

    public static OneOf<PaymentFrequency, IEnumerable<DomainRuleFault>> CreateMandatory(string value, IEnumerable<string> acceptableValues)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(PaymentFrequency)));
            return faults;
        }

        if (acceptableValues.Contains(value) is false)
        {
            faults.Add(new DomainRuleFault("{0} property has value '{1}' which is not currently in the list of available values.", nameof(PaymentFrequency), value));
        }
        
        return faults.Any() ? faults : new PaymentFrequency(value);
    }

    public static OneOf<Maybe<PaymentFrequency>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) =>
        CreateOptional(value, DefaultAcceptableValues);

    public static OneOf<Maybe<PaymentFrequency>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, IEnumerable<string> acceptableValues)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<PaymentFrequency>, IEnumerable<DomainRuleFault>>(Maybe<PaymentFrequency>.None);
        }

        return CreateMandatory(value, acceptableValues).Match(
            valueObject => new OneOf<Maybe<PaymentFrequency>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<PaymentFrequency>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static PaymentFrequency Convert(string value) =>
        CreateMandatory(value, new[] { value }).Match(
            valueObject => valueObject,
            fault => new PaymentFrequency(string.Empty)
        );
    
    public int CompareTo(PaymentFrequency? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}