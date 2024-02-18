using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Schema.Forms;
using Synonms.RestEasy.Sample.Api.Lookups;

namespace Synonms.RestEasy.Sample.Api.Infrastructure;

public class LookupOptionsProvider : ILookupOptionsProvider
{
    public IEnumerable<FormFieldOption> Get(string discriminator)
    {
        return discriminator switch
        {
            "Title" => new[]
            {
                new FormFieldOption("Dr"),
                new FormFieldOption("Miss"),
                new FormFieldOption("Mr"),
                new FormFieldOption("Mrs"),
                new FormFieldOption("Ms"),
                new FormFieldOption("Mx"),
                new FormFieldOption("Prof"),
                new FormFieldOption("Sir")
            },
            "Sex" => new[]
            {
                new FormFieldOption("Male"),
                new FormFieldOption("Female")
            },
            nameof(CurrencyLookup) => new[]
            {
                new FormFieldOption(CurrencyLookup.CurrencyLookupId1.Value) { Label = "GBP - Pounds Sterling" }
            },
            nameof(LeavingReasonLookup) => new[]
            {
                new FormFieldOption(LeavingReasonLookup.LeavingReasonLookupId1.Value) { Label = "LV01 - Reason 1" },
                new FormFieldOption(LeavingReasonLookup.LeavingReasonLookupId2.Value) { Label = "LV02 - Reason 2" }
            },
            _ => Enumerable.Empty<FormFieldOption>()
        };
    }
}