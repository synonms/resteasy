using Synonms.RestEasy.Core.Domain;

namespace Synonms.RestEasy.Sample.Api.Lookups;

public class CurrencyLookup : Lookup
{
    public static readonly EntityId<Lookup> CurrencyLookupId1 = EntityId<Lookup>.Parse("20000000-0000-0000-0000-000000000001");

    public static readonly CurrencyLookup CurrencyLookup1 = new(CurrencyLookupId1) { LookupCode = "GBP", LookupName = "Pounds Sterling" };

    public CurrencyLookup()
    {
    }
    
    public CurrencyLookup(EntityId<Lookup> id)
    {
        Id = id;
    }
}
