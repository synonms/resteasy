using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Application;

public interface IChildResourceMapper
{
    object? Map(object value);
}

public interface IChildResourceMapper<in TAggregateMember, out TChildResource> : IChildResourceMapper
    where TAggregateMember : AggregateMember<TAggregateMember>
    where TChildResource : ChildResource
{
    TChildResource? Map(TAggregateMember aggregateMember);
}