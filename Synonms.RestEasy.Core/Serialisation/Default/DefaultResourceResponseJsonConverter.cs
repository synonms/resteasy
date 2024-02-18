using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Core.Schema.Client;
using Synonms.RestEasy.Core.Schema.Errors;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.Core.Serialisation.Default;

public class DefaultResourceResponseJsonConverter<TResource> : JsonConverter<ResourceResponse<TResource>>
    where TResource : Resource
{
    public override ResourceResponse<TResource> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out JsonDocument? doc))
        {
            throw new JsonException("Failed to parse JsonDocument");
        }

        using JsonDocument jsonDocument = doc;

        if (jsonDocument.RootElement.TryGetProperty(DefaultPropertyNames.Errors, out JsonElement _))
        {
            ErrorCollectionDocument? errorCollectionDocument = JsonSerializer.Deserialize<ErrorCollectionDocument>(jsonDocument.RootElement.ToString(), options);

            if (errorCollectionDocument is null)
            {
                throw new JsonException($"Failed to deserialise {nameof(errorCollectionDocument)}.");
            }
            
            return new ResourceResponse<TResource>(errorCollectionDocument);
        }

        ResourceDocument<TResource>? resourceCollectionDocument = JsonSerializer.Deserialize<ResourceDocument<TResource>>(jsonDocument.RootElement.ToString(), options);

        if (resourceCollectionDocument is null)
        {
            throw new JsonException($"Failed to deserialise {nameof(ResourceDocument<TResource>)}.");
        }
            
        return new ResourceResponse<TResource>(resourceCollectionDocument);
    }

    public override void Write(Utf8JsonWriter writer, ResourceResponse<TResource> value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}