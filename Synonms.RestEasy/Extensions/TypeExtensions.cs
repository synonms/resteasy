using System.Collections;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Server;

namespace Synonms.RestEasy.Extensions;

public static class TypeExtensions
{
    private static readonly Dictionary<Type, string> ResourcePropertyTypes = new ()
    {
        [typeof(string)] = DataTypes.String,
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
    
    public static bool IsResource(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType is not null
        && type.BaseType.IsGenericType
        && type.BaseType.GetGenericTypeDefinition() == typeof(ServerResource<>);

    public static bool IsChildResource(this Type type) =>
        !type.IsInterface
        && !type.IsAbstract
        && type.BaseType is not null
        && type.BaseType.IsGenericType
        && type.BaseType.GetGenericTypeDefinition() == typeof(ServerChildResource<>);

    public static string GetResourcePropertyType(this Type type)
    {
        if (ResourcePropertyTypes.ContainsKey(type))
        {
            return ResourcePropertyTypes[type];
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
}