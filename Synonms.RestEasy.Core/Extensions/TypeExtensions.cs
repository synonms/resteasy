using System.Collections;
using Synonms.RestEasy.Core.Domain;

namespace Synonms.RestEasy.Core.Extensions;

public static class TypeExtensions
{
    public static Type? GetArrayOrEnumerableElementType(this Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        return type.IsEnumerable() ? type.GetGenericArguments().FirstOrDefault() : null;
    }

    public static Type? GetValueObjectValueType(this Type type) =>
        type.IsValueObject() 
            ? type.BaseType?.GetGenericArguments().FirstOrDefault()
            : null;
    
    public static bool IsAggregateRoot(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType is not null
        && type.BaseType.IsGenericType
        && (type.BaseType.GetGenericTypeDefinition() == typeof(AggregateRoot<>) 
            || type.BaseType.BaseType is not null && type.BaseType.BaseType.IsGenericType && type.BaseType.BaseType.GetGenericTypeDefinition() == typeof(AggregateRoot<>));

    public static bool IsAggregateMember(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType is not null
        && type.BaseType.IsGenericType
        && (type.BaseType.GetGenericTypeDefinition() == typeof(AggregateMember<>)
            || type.BaseType.BaseType is not null && type.BaseType.BaseType.IsGenericType && type.BaseType.BaseType.GetGenericTypeDefinition() == typeof(AggregateMember<>));

    public static bool IsArrayOrEnumerable(this Type type) =>
        type.IsArray || type.IsEnumerable();

    public static bool IsEnumerable(this Type type) =>
        type.IsGenericType && type.GetInterfaces().Any(x => x == typeof(IEnumerable));

    public static bool IsLookup(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType is not null
        && type.BaseType == typeof(Lookup);

    public static bool IsLookupId(this Type type) =>
        type == typeof(EntityId<Lookup>);

    public static bool IsValueObject(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType is not null
        && type.BaseType.IsGenericType
        && type.BaseType.GetGenericTypeDefinition() == typeof(ValueObject<>);

    public static bool IsEntityId(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.IsGenericType
        && type.GetGenericTypeDefinition() == typeof(EntityId<>);
}