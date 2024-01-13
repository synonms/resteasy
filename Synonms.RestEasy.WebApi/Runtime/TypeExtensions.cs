using System.Reflection;
using Synonms.RestEasy.Core.Extensions;
using Synonms.RestEasy.WebApi.Application;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Runtime;

public static class TypeExtensions
{
    private static readonly Dictionary<Type, string> TypeToDataTypeMappings = new ()
    {
        [typeof(string)] = DataTypes.String,
        [typeof(Guid)] = DataTypes.String,
        [typeof(bool)] = DataTypes.Boolean,
        [typeof(bool?)] = DataTypes.Boolean,
        [typeof(DateOnly)] = DataTypes.DateOnly,
        [typeof(DateOnly?)] = DataTypes.DateOnly,
        [typeof(TimeOnly)] = DataTypes.TimeOnly, 
        [typeof(TimeOnly?)] = DataTypes.TimeOnly,
        [typeof(DateTime)] = DataTypes.DateTime,
        [typeof(DateTime?)] = DataTypes.DateTime,
        [typeof(TimeSpan)] = DataTypes.Duration, 
        [typeof(TimeSpan?)] = DataTypes.Duration,
        [typeof(decimal)] = DataTypes.Decimal,
        [typeof(decimal?)] = DataTypes.Decimal, 
        [typeof(double)] = DataTypes.Number,
        [typeof(double?)] = DataTypes.Number,
        [typeof(float)] = DataTypes.Number,
        [typeof(float?)] = DataTypes.Number,
        [typeof(int)] = DataTypes.Integer,
        [typeof(int?)] = DataTypes.Integer,
        [typeof(uint)] = DataTypes.Integer,
        [typeof(uint?)] = DataTypes.Integer,
        [typeof(long)] = DataTypes.Integer,
        [typeof(long?)] = DataTypes.Integer,
        [typeof(ulong)] = DataTypes.Integer,
        [typeof(ulong?)] = DataTypes.Integer,
        [typeof(short)] = DataTypes.Integer,
        [typeof(short?)] = DataTypes.Integer,
        [typeof(ushort)] = DataTypes.Integer,
        [typeof(ushort?)] = DataTypes.Integer
    };
    
    public static string GetResourceDataType(this Type type)
    {
        if (TypeToDataTypeMappings.TryGetValue(type, out string? propertyType))
        {
            return propertyType;
        }

        if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            return DataTypes.Array;
        }

        if (type.IsEntityId())
        {
            return DataTypes.String;
        }
        
        return DataTypes.Object;
    }

    public static IEnumerable<PropertyInfo> GetPublicInstanceProperties(this Type type, string[] excludePropertyNames) =>
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(propertyInfo => excludePropertyNames.Contains(propertyInfo.Name) is false);
    
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

    public static bool IsResource(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType == typeof(Resource);

    public static bool IsChildResource(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType == typeof(ChildResource);

    public static bool IsLookupResource(this Type type) =>
        type == typeof(LookupResource);
}