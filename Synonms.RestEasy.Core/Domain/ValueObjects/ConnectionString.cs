using Synonms.Functional;
using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.ValueObjects;

public record ConnectionString : ValueObject<string>, IComparable, IComparable<ConnectionString>
{
    private ConnectionString(string value) : base(value)
    {
    }
    
    public static implicit operator string(ConnectionString valueObject) => valueObject.Value;

    public static OneOf<ConnectionString, IEnumerable<DomainRuleFault>> CreateMandatory(string value, int maxLength)
    {
        List<DomainRuleFault> faults = new ();

        if (string.IsNullOrWhiteSpace(value))
        {
            faults.Add(new DomainRuleFault("{0} property is required.", nameof(ConnectionString)));
            return faults;
        }

        if (value.Length > maxLength)
        {
            faults.Add(new DomainRuleFault("{0} property has maximum length of {1}.", nameof(ConnectionString), maxLength));
        }
        
        return faults.Any() ? faults : new ConnectionString(value);
    }
    
    public static OneOf<Maybe<ConnectionString>, IEnumerable<DomainRuleFault>> CreateOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new OneOf<Maybe<ConnectionString>, IEnumerable<DomainRuleFault>>(Maybe<ConnectionString>.None);
        }

        return CreateMandatory(value, maxLength).Match(
            valueObject => new OneOf<Maybe<ConnectionString>, IEnumerable<DomainRuleFault>>(valueObject), 
            faults => new OneOf<Maybe<ConnectionString>, IEnumerable<DomainRuleFault>>(faults));
    }
    
    internal static ConnectionString Convert(string value) =>
        CreateMandatory(value, value.Length).Match(
            valueObject => valueObject,
            fault => new ConnectionString(string.Empty)
        );
    
    public int CompareTo(ConnectionString? other) => string.Compare(Value, other?.Value, StringComparison.OrdinalIgnoreCase);
    
    public int CompareTo(object? obj) => Value.CompareTo(obj);
}