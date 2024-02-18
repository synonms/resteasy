using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Application;

public interface IResourceMapper
{
    object? Map(object value);
}

public interface IResourceMapper<in TAggregateRoot, out TResource> : IResourceMapper
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    TResource Map(TAggregateRoot aggregateRoot);
}