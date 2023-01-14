using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Extensions;

public static class TypeExtensions
{
    public static bool IsAggregateRoot(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType is not null
        && type.BaseType.IsGenericType
        && type.BaseType.GetGenericTypeDefinition() == (typeof(AggregateRoot<>));

    public static bool IsAggregateMember(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType is not null
        && type.BaseType.IsGenericType
        && type.BaseType.GetGenericTypeDefinition() == (typeof(AggregateMember<>));

    public static bool IsValueObject(this Type type) =>
        !type.IsInterface
            && !type.IsAbstract
            && type.BaseType is not null
            && type.BaseType.IsGenericType
            && type.BaseType.GetGenericTypeDefinition() == (typeof(ValueObject<>));

    public static bool IsEntityId(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.IsGenericType
        && type.GetGenericTypeDefinition() == typeof(EntityId<>);
    
    public static bool IsResource(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType is not null
        && type.BaseType.IsGenericType
        && type.BaseType.GetGenericTypeDefinition() == (typeof(Resource<>));
}