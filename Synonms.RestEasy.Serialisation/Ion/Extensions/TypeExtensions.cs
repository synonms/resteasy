using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema.Client;
using Synonms.RestEasy.Abstractions.Schema.Server;
using Synonms.RestEasy.SharedKernel.Extensions;

namespace Synonms.RestEasy.Serialisation.Ion.Extensions;

public static class TypeExtensions
{
    public static bool IsSerialisableEntityId(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.IsGenericType
        && type.GetGenericTypeDefinition() == typeof(EntityId<>);

    public static bool IsForRelatedEntityCollectionLink(this Type type) =>
        type.IsArrayOrEnumerable()
        && (type.GetArrayOrEnumerableElementType()?.IsEntityId() ?? false);

    public static bool IsSerialisableServerResource(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType is not null
        && type.BaseType.IsGenericType
        && type.BaseType.GetGenericTypeDefinition() == (typeof(ServerResource<>));

    public static bool IsSerialisableServerChildResource(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType is not null
        && type.BaseType.IsGenericType
        && type.BaseType.GetGenericTypeDefinition() == (typeof(ServerChildResource<>));
    
    public static bool IsSerialisableClientResource(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.IsAssignableTo(typeof(ClientResource));

    public static bool IsSerialisableClientChildResource(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.IsAssignableTo(typeof(ClientChildResource));
    
    private static bool IsEntityId(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.IsGenericType
        && type.GetGenericTypeDefinition() == typeof(EntityId<>);
}