using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
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

    public static bool IsForEmbeddedResource(this Type type) =>
        type.IsResource();

    public static bool IsForEmbeddedResourceCollection(this Type type) =>
        type.IsArrayOrEnumerable()
        && (type.GetArrayOrEnumerableElementType()?.IsResource() ?? false);

    public static bool IsForEmbeddedChildResourceCollection(this Type type) =>
        type.IsArrayOrEnumerable()
        && (type.GetArrayOrEnumerableElementType()?.IsChildResource() ?? false);

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
    
    public static bool IsResource(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType == typeof(Resource);

    public static bool IsChildResource(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType == typeof(ChildResource);
}