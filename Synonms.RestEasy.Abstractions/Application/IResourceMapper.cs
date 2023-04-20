using Microsoft.AspNetCore.Http;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Abstractions.Application;

public interface IResourceMapper<in TAggregateRoot, out TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    TResource Map(HttpContext httpContext, TAggregateRoot aggregateRoot);
}