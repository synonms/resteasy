using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Client;
using Synonms.RestEasy.Serialisation.Ion.Constants;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonClientResourceCollectionResponseJsonConverter<TResource> : JsonConverter<ClientResourceCollectionResponse<TResource>>
    where TResource : ClientResource
{
    public override ClientResourceCollectionResponse<TResource> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            
            return new ClientResourceCollectionResponse<TResource>(errorCollectionDocument);
        }

        ClientResourceCollectionDocument<TResource>? resourceCollectionDocument = JsonSerializer.Deserialize<ClientResourceCollectionDocument<TResource>>(jsonDocument.RootElement.ToString(), options);

        if (resourceCollectionDocument is null)
        {
            throw new JsonException($"Failed to deserialise {nameof(ClientResourceCollectionDocument<TResource>)}.");
        }
            
        return new ClientResourceCollectionResponse<TResource>(resourceCollectionDocument);
    }

    public override void Write(Utf8JsonWriter writer, ClientResourceCollectionResponse<TResource> value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}