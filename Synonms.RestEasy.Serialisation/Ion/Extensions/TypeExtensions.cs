using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Client;
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

    public static bool IsSerialisableResource(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType == (typeof(Resource));

    public static bool IsSerialisableChildResource(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType == (typeof(ChildResource));
    
    private static bool IsEntityId(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.IsGenericType
        && type.GetGenericTypeDefinition() == typeof(EntityId<>);
}