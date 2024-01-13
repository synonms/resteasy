using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Title : ValueObject<string>, IComparable, IComparable<Title>
{
    public static readonly string[] DefaultAcceptableValues = 
    {
        "Dr",
        "Miss",
        "Mr",
        "Mrs",
        "Ms",
        "Mx",
        "Prof",
        "Sir"
    };
    
    private Title(string value) : base(value)
    {
    }
    
    public static implicit operator string(Title valueObject) => valueObject.Value;

    public static OneOf<Title, IEnumerable<DomainRuleFault>> CreateMandatory(string value) =>
        CreateMandatory(value, DefaultAcceptableValues);

    public static OneOf<Title, IEnumerable<DomainRuleFault>> CreateMandatory(string value, IEnumerable<string> acceptableValues)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(Title)));
            return faults;
        }

        if (acceptableValues.Contains(value) is false)
        {
            faults.Add(new DomainRuleFault("{0} property has value '{1}' which is not currently in the list of available values.", nameof(Title), value));
        }
        
        return faults.Any() ? faults : new Title(value);
    }

    public static OneOf<Maybe<Title>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) =>
        CreateOptional(value, DefaultAcceptableValues);

    public static OneOf<Maybe<Title>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, IEnumerable<string> acceptableValues)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<Title>, IEnumerable<DomainRuleFault>>(Maybe<Title>.None);
        }

        return CreateMandatory(value, acceptableValues).Match(
            valueObject => new OneOf<Maybe<Title>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Title>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Title Convert(string value) =>
        CreateMandatory(value, new[] { value }).Match(
            valueObject => valueObject,
            fault => new Title(string.Empty)
        );
    
    public int CompareTo(Title? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}