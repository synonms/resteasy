using Synonms.RestEasy.Core.Domain.Faults;

namespace Synonms.RestEasy.Core.Domain.Rules.Rulesets;

public class RelatedEntityIdRuleset<TEntity> : IDomainRuleset
    where TEntity : Entity<TEntity>
{
    private readonly string _propertyName;
    private readonly EntityId<TEntity> _value;

    private RelatedEntityIdRuleset(string propertyName, EntityId<TEntity> value)
    {
        _propertyName = propertyName;
        _value = value;
    }

    public IEnumerable<DomainRuleFault> Apply() =>
        DomainRules.CreateBuilder()
            .Property(_propertyName, _value)
            .AddRule(payFrequency => payFrequency.IsEmpty, "Related Entity Id is required.", _propertyName)
            .Build();

    public static RelatedEntityIdRuleset<TEntity> Create(string propertyName, EntityId<TEntity> value) => 
        new (propertyName, value);
}