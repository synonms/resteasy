using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Abstractions.Domain;

public interface ILookupOptionsProvider
{
    IEnumerable<FormFieldOption> Get(string discriminator);
}