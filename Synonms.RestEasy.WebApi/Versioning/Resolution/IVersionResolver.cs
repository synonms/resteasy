using Microsoft.AspNetCore.Http;

namespace Synonms.RestEasy.WebApi.Versioning.Resolution;

public interface IVersionResolver
{
    int Resolve(HttpRequest httpRequest);
}