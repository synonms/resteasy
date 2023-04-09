using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Schema.Client;
using Synonms.RestEasy.Abstractions.Schema.Server;
using Synonms.RestEasy.Serialisation.Ion.Extensions;

namespace Synonms.RestEasy.Serialisation.Ion
{
    public class IonCustomJsonConverterFactory : JsonConverterFactory
    {
        private readonly Dictionary<Type, Type> _supportedServerTypes = new ()
        {
            { typeof(ServerResourceDocument<,>), typeof(IonServerResourceDocumentJsonConverter<,>) },
            { typeof(ServerResourceCollectionDocument<,>), typeof(IonServerResourceCollectionDocumentJsonConverter<,>) }
        };

        private readonly Dictionary<Type, Type> _supportedClientTypes = new ()
        {
            { typeof(ClientResourceDocument<>), typeof(IonClientResourceDocumentJsonConverter<>) },
            { typeof(ClientResourceCollectionDocument<>), typeof(IonClientResourceCollectionDocumentJsonConverter<>) },
            { typeof(ClientResourceResponse<>), typeof(IonClientResourceResponseJsonConverter<>) },
            { typeof(ClientResourceCollectionResponse<>), typeof(IonClientResourceCollectionResponseJsonConverter<>) }
        };

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsSerialisableEntityId())
            {
                return true;
            }

            if (typeToConvert.IsSerialisableServerResource())
            {
                return true;
            }

            if (typeToConvert.IsSerialisableServerChildResource())
            {
                return true;
            }

            if (typeToConvert.IsSerialisableClientResource())
            {
                return true;
            }

            if (typeToConvert.IsSerialisableClientChildResource())
            {
                return true;
            }

            if (typeToConvert.IsGenericType is false)
            {
                return false;
            }

            Type genericType = typeToConvert.GetGenericTypeDefinition();

            return _supportedServerTypes.ContainsKey(genericType) || _supportedClientTypes.ContainsKey(genericType);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (CanConvert(typeToConvert) is false)
            {
                return null;
            }

            if (typeToConvert.IsSerialisableEntityId())
            {
                return CreateEntityIdConverter(typeToConvert, options);
            }

            if (typeToConvert.IsSerialisableServerResource())
            {
                return CreateServerResourceConverter(typeToConvert, options);
            }

            if (typeToConvert.IsSerialisableServerChildResource())
            {
                return CreateServerChildResourceConverter(typeToConvert, options);
            }

            if (typeToConvert.IsSerialisableClientResource())
            {
                return CreateClientResourceConverter(typeToConvert, options);
            }

            if (typeToConvert.IsSerialisableClientChildResource())
            {
                return CreateClientChildResourceConverter(typeToConvert, options);
            }

            Type genericType = typeToConvert.GetGenericTypeDefinition();
            Type resourceType = typeToConvert.GetGenericArguments().Last();

            if (_supportedServerTypes.ContainsKey(genericType))
            {
                Type aggregateRootType = typeToConvert.GetGenericArguments().First();
                Type serverConverterType = _supportedServerTypes[genericType].MakeGenericType(aggregateRootType, resourceType);

                return (JsonConverter?)Activator.CreateInstance(serverConverterType);
            }
            
            Type clientConverterType = _supportedClientTypes[genericType].MakeGenericType(resourceType);

            return (JsonConverter?)Activator.CreateInstance(clientConverterType);
        }

        private static JsonConverter? CreateServerResourceConverter(Type resourceType, JsonSerializerOptions options)
        {
            Type? aggregateRootType = resourceType.BaseType?.GetGenericArguments().First();

            if (aggregateRootType is null)
            {
                throw new InvalidOperationException($"Type '{resourceType}' is considered a derivative of Resource<> but the TAggregateRoot generic type parameter cannot be determined.");
            }
            
            Type converterType = typeof(IonServerResourceJsonConverter<,>).MakeGenericType(aggregateRootType, resourceType);
                
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }

        private static JsonConverter? CreateServerChildResourceConverter(Type childResourceType, JsonSerializerOptions options)
        {
            Type? aggregateMemberType = childResourceType.BaseType?.GetGenericArguments().First();

            if (aggregateMemberType is null)
            {
                throw new InvalidOperationException($"Type '{childResourceType}' is considered a derivative of ChildResource<> but the TAggregateMember generic type parameter cannot be determined.");
            }
            
            Type converterType = typeof(IonServerChildResourceJsonConverter<,>).MakeGenericType(aggregateMemberType, childResourceType);
                
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }

        private static JsonConverter? CreateClientResourceConverter(Type resourceType, JsonSerializerOptions options)
        {
            Type converterType = typeof(IonClientResourceJsonConverter<>).MakeGenericType(resourceType);
                
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }

        private static JsonConverter? CreateClientChildResourceConverter(Type childResourceType, JsonSerializerOptions options)
        {
            Type converterType = typeof(IonClientChildResourceJsonConverter<>).MakeGenericType(childResourceType);
                
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