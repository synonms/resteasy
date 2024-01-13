using Synonms.RestEasy.WebApi.Attributes;
using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Domain.Rules;
using Synonms.RestEasy.Core.Domain.Rules.Rulesets;
using Synonms.RestEasy.Core.Domain.ValueObjects;
using Synonms.RestEasy.Sample.Api.Lookups;

namespace Synonms.RestEasy.Sample.Api.Legislations;

[RestEasyResource(typeof(LegislationResource), "legislations", requiresAuthentication: false, isCreateDisabled: true, isUpdateDisabled: true, isDeleteDisabled: true)]
public class Legislation : AggregateRoot<Legislation>
{
    public const int NameMaxLength = 40;

    private Legislation()
    {
    }
    
    private Legislation(EntityId<Legislation> id, Moniker name, EntityId<Lookup> currencyId)
        : this(name, currencyId)
    {
        Id = id;
    }
    
    private Legislation(Moniker name, EntityId<Lookup> currencyId)
    {
        Name = name;
        CurrencyId = currencyId;
    }
    
    public Moniker Name { get; private set; } = null!;

    public EntityId<Lookup> CurrencyId { get; private set; } = null!;
    public CurrencyLookup Currency { get; private set; } = null!;

    internal static Result<Legislation> Create(LegislationResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Name, x => Moniker.CreateMandatory(x, NameMaxLength), out Moniker nameValueObject)
            .WithDomainRules(
                RelatedEntityIdRuleset<Lookup>.Create(nameof(CurrencyId), resource.CurrencyId)
                )
            .Build()
            .ToResult(() => new Legislation((EntityId<Legislation>)resource.Id, nameValueObject, resource.CurrencyId));

    internal Maybe<Fault> Update(LegislationResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Name, x => Moniker.CreateMandatory(x, NameMaxLength), out Moniker nameValueObject)
            .WithDomainRules(
                RelatedEntityIdRuleset<Lookup>.Create(nameof(CurrencyId), resource.CurrencyId)
            )
            .Build()
            .BiBind(() =>
            {
                UpdateMandatoryValue(_ => _.Name, nameValueObject);
                UpdateMandatoryValue(_ => _.CurrencyId, resource.CurrencyId);

                return Maybe<Fault>.None;
            });
}