using Synonms.RestEasy.Core.Schema.Forms;

namespace Synonms.RestEasy.Core.Domain;

public interface ILookupOptionsProvider
{
    IEnumerable<FormFieldOption> Get(string discriminator);
}