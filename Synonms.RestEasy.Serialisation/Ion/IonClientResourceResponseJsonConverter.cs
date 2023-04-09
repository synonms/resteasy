using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Client;
using Synonms.RestEasy.Serialisation.Ion.Constants;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonClientResourceResponseJsonConverter<TResource> : JsonConverter<ClientResourceResponse<TResource>>
    where TResource : ClientResource
{
    public override ClientResourceResponse<TResource> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out JsonDocument? doc))
        {
            throw new JsonException("Failed to parse JsonDocument");
        }

        using JsonDocument jsonDocument = doc;

        if (jsonDocument.RootElement.TryGetProperty(IonPropertyNames.Errors, out JsonElement _))
        {
            ErrorCollectionDocument? errorCollectionDocument = JsonSerializer.Deserialize<ErrorCollectionDocument>(jsonDocument.RootElement.ToString(), options);

            if (errorCollectionDocument is null)
            {
                throw new JsonException($"Failed to deserialise {nameof(errorCollectionDocument)}.");
            }
            
            return new ClientResourceResponse<TResource>(errorCollectionDocument);
        }

        ClientResourceDocument<TResource>? resourceCollectionDocument = JsonSerializer.Deserialize<ClientResourceDocument<TResource>>(jsonDocument.RootElement.ToString(), options);

        if (resourceCollectionDocument is null)
        {
            throw new JsonException($"Failed to deserialise {nameof(ClientResourceDocument<TResource>)}.");
        }
            
        return new ClientResourceResponse<TResource>(resourceCollectionDocument);
    }

    public override void Write(Utf8JsonWriter writer, ClientResourceResponse<TResource> value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}