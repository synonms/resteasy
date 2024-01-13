using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record Hours : ValueObject<int>, IComparable, IComparable<Hours>
{
    private const int DefaultMinValue = 0;
    private const int DefaultMaxValue = int.MaxValue;

    private Hours(int value) : base(value)
    {
    }
    
    public static implicit operator int(Hours valueObject) => valueObject.Value;

    public static OneOf<Hours, IEnumerable<DomainRuleFault>> CreateMandatory(int value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Hours, IEnumerable<DomainRuleFault>> CreateMandatory(int value, int minValue, int maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(Hours), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(Hours), maxValue));
        }

        return faults.Any() ? faults : new Hours(value);
    }

    public static OneOf<Maybe<Hours>, IEnumerable<DomainRuleFault>> CreateOptional(int? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<Maybe<Hours>, IEnumerable<DomainRuleFault>> CreateOptional(int? value, int minValue, int maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<Hours>, IEnumerable<DomainRuleFault>>(Maybe<Hours>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<Hours>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<Hours>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static Hours Convert(int value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new Hours(int.MinValue)
        );
    
    public int CompareTo(Hours? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}