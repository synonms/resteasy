using Microsoft.AspNetCore.Http;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Abstractions.Application;

public interface IChildResourceMapper
{
    object? Map(HttpContext httpContext, object value);
}

public interface IChildResourceMapper<in TAggregateMember, out TChildResource> : IChildResourceMapper
    where TAggregateMember : AggregateMember<TAggregateMember>
    where TChildResource : ChildResource<TAggregateMember>
{
    TChildResource? Map(HttpContext httpContext, TAggregateMember aggregateMember);
}