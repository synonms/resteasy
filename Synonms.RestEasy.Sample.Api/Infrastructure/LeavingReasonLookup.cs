using Synonms.RestEasy.Core.Domain;

namespace Synonms.RestEasy.Sample.Api.Infrastructure;

public class LeavingReasonLookup : Lookup
{
    public static readonly EntityId<Lookup> LeavingReasonLookupId1 = EntityId<Lookup>.Parse("10000000-0000-0000-0000-000000000001");
    public static readonly EntityId<Lookup> LeavingReasonLookupId2 = EntityId<Lookup>.Parse("10000000-0000-0000-0000-000000000002");

    public static readonly LeavingReasonLookup LeavingReasonLookup1 = new(LeavingReasonLookupId1) { LookupCode = "LV01", LookupName = "Reason 1" };
    public static readonly LeavingReasonLookup LeavingReasonLookup2 = new(LeavingReasonLookupId2) { LookupCode = "LV02", LookupName = "Reason 2" };

    public LeavingReasonLookup()
    {
    }
    
    public LeavingReasonLookup(EntityId<Lookup> id)
    {
        Id = id;
    }
}
