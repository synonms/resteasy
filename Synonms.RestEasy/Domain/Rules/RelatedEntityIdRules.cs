using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Domain.Rules;

public class RelatedEntityIdRules<TEntity> : IDomainRuleset
    where TEntity : Entity<TEntity>
{
    private readonly string _propertyName;
    private readonly EntityId<TEntity> _value;

    private RelatedEntityIdRules(string propertyName, EntityId<TEntity> value)
    {
        _propertyName = propertyName;
        _value = value;
    }

    public IEnumerable<DomainRuleFault> Apply() =>
        DomainRules.CreateBuilder()
            .Property(_propertyName, _value)
            .AddRule(payFrequency => payFrequency.IsEmpty, "Related Entity Id is required.", _propertyName)
            .Build();

    public static RelatedEntityIdRules<TEntity> Create(string propertyName, EntityId<TEntity> value) => 
        new (propertyName, value);
}