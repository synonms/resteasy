using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Persistence;
using Synonms.RestEasy.Sample.Api.Lookups;

namespace Synonms.RestEasy.Sample.Api.Infrastructure;

public class CurrencyLookupRepository : ILookupRepository<CurrencyLookup>
{
    private readonly Dictionary<EntityId<Lookup>, CurrencyLookup> _currencyLookups = new()
    {
        [CurrencyLookup.CurrencyLookupId1] = CurrencyLookup.CurrencyLookup1
    };

    public Task<CurrencyLookup?> FindAsync(EntityId<Lookup> id) =>
        Task.FromResult(_currencyLookups.TryGetValue(id, out CurrencyLookup? currencyLookup) ? currencyLookup : null);

    public Task<IEnumerable<CurrencyLookup>> ReadAsync() =>
        Task.FromResult(_currencyLookups.Values.AsEnumerable());
}