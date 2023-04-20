using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Forms;

namespace Synonms.RestEasy.Sample.Api.Hacks;

public class LookupOptionsProvider : ILookupOptionsProvider
{
    public IEnumerable<FormFieldOption> Get(string discriminator) =>
        discriminator switch
        {
            "Colour" => new FormFieldOption[]
            {
                new("Black"),
                new("Blue"),
                new("Brown"),
                new("Green"),
                new("Purple"),
                new("Orange"),
                new("Red"),
                new("White"),
                new("Yellow")
            },
            _ => Array.Empty<FormFieldOption>()
        };
}