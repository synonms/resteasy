using Synonms.RestEasy.WebApi.Schema.Forms;

namespace Synonms.RestEasy.WebApi.Domain;

public interface ILookupOptionsProvider
{
    IEnumerable<FormFieldOption> Get(string discriminator);
}