using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Extensions;

namespace Synonms.RestEasy.Serialisation.Ion
{
    public class IonCustomJsonConverterFactory : JsonConverterFactory
    {
        private readonly Dictionary<Type, Type> _supportedTypes = new ()
        {
            { typeof(ResourceDocument<,>), typeof(IonResourceDocumentJsonConverter<,>) },
            { typeof(ResourceCollectionDocument<,>), typeof(IonResourceCollectionDocumentJsonConverter<,>) }
        };
        
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsEntityId())
            {
                return true;
            }

            if (typeToConvert.IsResource())
            {
                return true;
            }

            if (typeToConvert.IsChildResource())
            {
                return true;
            }

            if (typeToConvert.IsGenericType is false)
            {
                return false;
            }

            Type genericType = typeToConvert.GetGenericTypeDefinition();

            return _supportedTypes.ContainsKey(genericType);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (CanConvert(typeToConvert) is false)
            {
                return null;
            }

            if (typeToConvert.IsEntityId())
            {
                return CreateEntityIdConverter(typeToConvert, options);
            }

            if (typeToConvert.IsResource())
            {
                return CreateResourceConverter(typeToConvert, options);
            }

            if (typeToConvert.IsChildResource())
            {
                return CreateChildResourceConverter(typeToConvert, options);
            }

            Type genericType = typeToConvert.GetGenericTypeDefinition();
            Type aggregateRootType = typeToConvert.GetGenericArguments().First();
            Type resourceType = typeToConvert.GetGenericArguments().Last();
            Type converterType = _supportedTypes[genericType].MakeGenericType(aggregateRootType, resourceType);

            return (JsonConverter?)Activator.CreateInstance(converterType);
        }

        private static JsonConverter? CreateResourceConverter(Type resourceType, JsonSerializerOptions options)
        {
            Type? aggregateRootType = resourceType.BaseType?.GetGenericArguments().First();

            if (aggregateRootType is null)
            {
                throw new InvalidOperationException($"Type '{resourceType}' is considered a derivative of Resource<> but the TAggregateRoot generic type parameter cannot be determined.");
            }
            
            Type converterType = typeof(IonResourceJsonConverter<,>).MakeGenericType(aggregateRootType, resourceType);
                
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }

        private static JsonConverter? CreateChildResourceConverter(Type childResourceType, JsonSerializerOptions options)
        {
            Type? aggregateMemberType = childResourceType.BaseType?.GetGenericArguments().First();

            if (aggregateMemberType is null)
            {
                throw new InvalidOperationException($"Type '{childResourceType}' is considered a derivative of ChildResource<> but the TAggregateMember generic type parameter cannot be determined.");
            }
            
            Type converterType = typeof(IonChildResourceJsonConverter<,>).MakeGenericType(aggregateMemberType, childResourceType);
                
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }

        private static JsonConverter? CreateEntityIdConverter(Type entityIdType, JsonSerializerOptions options)
        {
            Type? entityType = entityIdType.GetGenericArguments().First();

            if (entityType is null)
            {
                throw new InvalidOperationException($"Type '{entityIdType}' is considered an EntityId<> but the TEntity generic type parameter cannot be determined.");
            }
            
            Type converterType = typeof(IonEntityIdJsonConverter<>).MakeGenericType(entityType);
                
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }
    }
}