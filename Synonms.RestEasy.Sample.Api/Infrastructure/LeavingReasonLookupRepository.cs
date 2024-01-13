using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Persistence;

namespace Synonms.RestEasy.Sample.Api.Infrastructure;

public class LeavingReasonLookupRepository : ILookupRepository<LeavingReasonLookup>
{
    private readonly Dictionary<EntityId<Lookup>, LeavingReasonLookup> _leavingReasonLookups = new()
    {
        [LeavingReasonLookup.LeavingReasonLookupId1] = LeavingReasonLookup.LeavingReasonLookup1,
        [LeavingReasonLookup.LeavingReasonLookupId2] = LeavingReasonLookup.LeavingReasonLookup2
    };

    public Task<LeavingReasonLookup?> FindAsync(EntityId<Lookup> id) =>
        Task.FromResult(_leavingReasonLookups.TryGetValue(id, out LeavingReasonLookup? leavingReasonLookup) ? leavingReasonLookup : null);

    public Task<IEnumerable<LeavingReasonLookup>> ReadAsync() =>
        Task.FromResult(_leavingReasonLookups.Values.AsEnumerable());
}