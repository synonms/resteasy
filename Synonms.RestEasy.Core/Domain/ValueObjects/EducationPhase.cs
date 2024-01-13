using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record EducationPhase : ValueObject<string>, IComparable, IComparable<EducationPhase>
{
    public static readonly string[] DefaultAcceptableValues = 
    {
        "Early Years",
        "Primary",
        "Secondary",
        "Further Education",
        "Higher Education"
    };
    
    private EducationPhase(string value) : base(value)
    {
    }
    
    public static implicit operator string(EducationPhase valueObject) => valueObject.Value;

    public static OneOf<EducationPhase, IEnumerable<DomainRuleFault>> CreateMandatory(string value) =>
        CreateMandatory(value, DefaultAcceptableValues);

    public static OneOf<EducationPhase, IEnumerable<DomainRuleFault>> CreateMandatory(string value, IEnumerable<string> acceptableValues)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(EducationPhase)));
            return faults;
        }

        if (acceptableValues.Contains(value) is false)
        {
            faults.Add(new DomainRuleFault("{0} property has value '{1}' which is not currently in the list of available values.", nameof(EducationPhase), value));
        }
        
        return faults.Any() ? faults : new EducationPhase(value);
    }

    public static OneOf<Maybe<EducationPhase>, IEnumerable<DomainRuleFault>> CreateOptional(string? value) =>
        CreateOptional(value, DefaultAcceptableValues);

    public static OneOf<Maybe<EducationPhase>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, IEnumerable<string> acceptableValues)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<EducationPhase>, IEnumerable<DomainRuleFault>>(Maybe<EducationPhase>.None);
        }

        return CreateMandatory(value, acceptableValues).Match(
            valueObject => new OneOf<Maybe<EducationPhase>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<EducationPhase>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static EducationPhase Convert(string value) =>
        CreateMandatory(value, new[] { value }).Match(
            valueObject => valueObject,
            fault => new EducationPhase(string.Empty)
        );
    
    public int CompareTo(EducationPhase? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}