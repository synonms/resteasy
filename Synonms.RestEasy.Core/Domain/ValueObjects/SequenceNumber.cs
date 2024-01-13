using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record SequenceNumber : ValueObject<int>, IComparable, IComparable<SequenceNumber>
{
    private const int DefaultMinValue = 1;
    private const int DefaultMaxValue = int.MaxValue;
    
    private SequenceNumber(int value) : base(value)
    {
    }
    
    public static implicit operator int(SequenceNumber valueObject) => valueObject.Value;

    public static OneOf<SequenceNumber, IEnumerable<DomainRuleFault>> CreateMandatory(int value) =>
        CreateMandatory(value, DefaultMinValue, DefaultMaxValue);

    public static OneOf<SequenceNumber, IEnumerable<DomainRuleFault>> CreateMandatory(int value, int minValue, int maxValue)
    {
        List<DomainRuleFault> faults = new ();

        if (value < minValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MinValueError, nameof(SequenceNumber), minValue));
        }

        if (value > maxValue)
        {
            faults.Add(new DomainRuleFault(ErrorTemplates.MaxValueError, nameof(SequenceNumber), maxValue));
        }

        return faults.Any() ? faults : new SequenceNumber(value);
    }

    public static OneOf<Maybe<SequenceNumber>, IEnumerable<DomainRuleFault>> CreateOptional(int? value) =>
        CreateOptional(value, DefaultMinValue, DefaultMaxValue);
    
    public static OneOf<Maybe<SequenceNumber>, IEnumerable<DomainRuleFault>> CreateOptional(int? value, int minValue, int maxValue)
    {
        if (value is null)
        {
            return new OneOf<Maybe<SequenceNumber>, IEnumerable<DomainRuleFault>>(Maybe<SequenceNumber>.None);
        }

        return CreateMandatory(value.Value, minValue, maxValue).Match(
            valueObject => new OneOf<Maybe<SequenceNumber>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<SequenceNumber>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static SequenceNumber Convert(int value) =>
        CreateMandatory(value, value, value).Match(
            valueObject => valueObject,
            fault => new SequenceNumber(int.MinValue));
    
    public int CompareTo(SequenceNumber? other) => Value.CompareTo(other?.Value);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}