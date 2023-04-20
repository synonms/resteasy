using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Forms;

namespace Synonms.RestEasy.Abstractions.Domain;

public interface ILookupOptionsProvider
{
    IEnumerable<FormFieldOption> Get(string discriminator);
}