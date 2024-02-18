using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Application;

public static class LookupResourceMapper
{
    public static LookupResource Map(Lookup lookup) =>
        new()
        {
            Id = lookup.Id.Value,
            LookupCode = lookup.LookupCode,
            LookupName = lookup.LookupName,
        };
    
    public static LookupResource? MapOptional(Lookup? lookup) =>
        lookup is null
            ? null
            : new LookupResource
            {
                Id = lookup.Id.Value,
                LookupCode = lookup.LookupCode,
                LookupName = lookup.LookupName,
            };
}