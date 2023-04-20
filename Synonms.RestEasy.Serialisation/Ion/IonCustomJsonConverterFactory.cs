using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Schema.Client;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Serialisation.Ion.Extensions;

namespace Synonms.RestEasy.Serialisation.Ion
{
    public class IonCustomJsonConverterFactory : JsonConverterFactory
    {
        private readonly Dictionary<Type, Type> _supportedGenericConverterTypes = new ()
        {
            { typeof(ResourceDocument<>), typeof(IonResourceDocumentJsonConverter<>) },
            { typeof(ResourceCollectionDocument<>), typeof(IonResourceCollectionDocumentJsonConverter<>) },
            { typeof(ResourceResponse<>), typeof(IonResourceResponseJsonConverter<>) },
            { typeof(ResourceCollectionResponse<>), typeof(IonResourceCollectionResponseJsonConverter<>) }
        };

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert.IsSerialisableEntityId())
            {
                return true;
            }

            if (typeToConvert.IsSerialisableResource())
            {
                return true;
            }

            if (typeToConvert.IsSerialisableChildResource())
            {
                return true;
            }

            if (typeToConvert.IsGenericType is false)
            {
                return false;
            }

            Type genericType = typeToConvert.GetGenericTypeDefinition();

            return _supportedGenericConverterTypes.ContainsKey(genericType);
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

            if (typeToConvert.IsSerialisableResource())
            {
                return CreateResourceConverter(typeToConvert);
            }

            if (typeToConvert.IsSerialisableChildResource())
            {
                return CreateChildResourceConverter(typeToConvert);
            }

            Type genericType = typeToConvert.GetGenericTypeDefinition();
            Type resourceType = typeToConvert.GetGenericArguments().Last();

            if (_supportedGenericConverterTypes.ContainsKey(genericType))
            {
                Type serverConverterType = _supportedGenericConverterTypes[genericType].MakeGenericType(resourceType);

                return (JsonConverter?)Activator.CreateInstance(serverConverterType);
            }

            return null;
        }

        private static JsonConverter? CreateResourceConverter(Type resourceType)
        {
            Type converterType = typeof(IonResourceJsonConverter<>).MakeGenericType(resourceType);
                
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }

        private static JsonConverter? CreateChildResourceConverter(Type childResourceType)
        {
            Type converterType = typeof(IonChildResourceJsonConverter<>).MakeGenericType(childResourceType);
                
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