using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Sample.Api.Hacks;

public class LookupOptionsProvider : ILookupOptionsProvider
{
    public IEnumerable<FormFieldOption> Get(string discriminator)
    {
        throw new NotImplementedException();
    }
}