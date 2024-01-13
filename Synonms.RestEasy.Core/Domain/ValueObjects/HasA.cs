using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record HasA : ValueObject<bool>, IComparable, IComparable<HasA>
{
    private HasA(bool value) : base(value)
    {
    }
    
    public static implicit operator bool(HasA valueObject) => valueObject.Value;

    public static OneOf<HasA, IEnumerable<DomainRuleFault>> CreateMandatory(bool value) => new HasA(value);

    public static OneOf<Maybe<HasA>, IEnumerable<DomainRuleFault>> CreateOptional(bool? value)
    {
        if (value is null)
        {
            return new OneOf<Maybe<HasA>, IEnumerable<DomainRuleFault>>(Maybe<HasA>.None);
        }

        return CreateMandatory(value.Value).Match(
            valueObject => new OneOf<Maybe<HasA>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<HasA>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static HasA Convert(bool value) =>
        CreateMandatory(value).Match(
            valueObject => valueObject,
            fault => new HasA(false)
        );

    public int CompareTo(HasA? other)
    {
        if (other is null)
        {
            return 1;
        }
        
        if (Value)
        {
            return other.Value ? 0 : 1;
        }
        
        if (other.Value) return -1;
        
        return 0;
    }
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}