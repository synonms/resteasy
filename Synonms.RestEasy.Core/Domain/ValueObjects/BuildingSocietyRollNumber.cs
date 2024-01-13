using System.Text.RegularExpressions;
using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record BuildingSocietyRollNumber : ValueObject<string>, IComparable, IComparable<BuildingSocietyRollNumber>
{
    public const string Pattern = @"^([a-zA-Z0-9\/.-]){1,18}$";

    private BuildingSocietyRollNumber(string value) : base(value)
    {
    }
    
    public static implicit operator string(BuildingSocietyRollNumber valueObject) => valueObject.Value;

    public static OneOf<BuildingSocietyRollNumber, IEnumerable<DomainRuleFault>> CreateMandatory(string value)
    {
        List<DomainRuleFault> faults = new ();

        if (Regex.IsMatch(value, Pattern) is false)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.PatternError, nameof(BuildingSocietyRollNumber), Pattern));
        }

        return faults.Any() ? faults : new BuildingSocietyRollNumber(value);
    }
    
    public static OneOf<Maybe<BuildingSocietyRollNumber>, IEnumerable<DomainRuleFault>> CreateOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<BuildingSocietyRollNumber>, IEnumerable<DomainRuleFault>>(Maybe<BuildingSocietyRollNumber>.None);
        }

        return CreateMandatory(value).Match(
            valueObject => new OneOf<Maybe<BuildingSocietyRollNumber>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<BuildingSocietyRollNumber>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static BuildingSocietyRollNumber Convert(string value) =>
        CreateMandatory(value).Match(
            valueObject => valueObject,
            fault => new BuildingSocietyRollNumber(string.Empty));

    public int CompareTo(BuildingSocietyRollNumber? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}