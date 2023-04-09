using Microsoft.AspNetCore.Http;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Server;

namespace Synonms.RestEasy.Abstractions.Application;

public interface IResourceMapper<in TAggregateRoot, out TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : ServerResource<TAggregateRoot>
{
    TResource Map(HttpContext httpContext, TAggregateRoot aggregateRoot);
}