using Synonms.RestEasy.Core.Attributes;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Schema;
using Synonms.RestEasy.Core.Schema.Resources;
using Synonms.RestEasy.Sample.Api.Lookups;

namespace Synonms.RestEasy.Sample.Api.Legislations;

public class LegislationResource : Resource
{
    public LegislationResource()
    {
    }
    
    public LegislationResource(Guid id, Link selfLink) : base(id, selfLink)
    {
    }
    
    [RestEasyRequired]    
    [RestEasyMaxLength(Legislation.NameMaxLength)]
    public string Name { get; set; } = string.Empty;
    
    [RestEasyRequired]
    [RestEasyLookup(nameof(CurrencyLookup))]
    public EntityId<Lookup> CurrencyId { get; set; } = EntityId<Lookup>.Uninitialised;

    [RestEasyHidden] 
    public LookupResource Currency { get; set; } = new();
}