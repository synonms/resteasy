using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Client;
using Synonms.RestEasy.Serialisation.Ion.Constants;
using Synonms.RestEasy.Serialisation.Ion.Extensions;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonClientResourceDocumentJsonConverter<TResource> : JsonConverter<ClientResourceDocument<TResource>>
    where TResource : ClientResource
{
    public override ClientResourceDocument<TResource> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out JsonDocument? doc))
        {
            throw new JsonException("Failed to parse JsonDocument");
        }

        using JsonDocument jsonDocument = doc;

        JsonElement valueElement = jsonDocument.RootElement.GetProperty(IonPropertyNames.Value);

        TResource? clientResource = JsonSerializer.Deserialize<TResource>(valueElement.ToString(), options);

        if (clientResource is null)
        {
            throw new JsonException($"Unable to deserialise client resource type {nameof(TResource)}.");
        }

        JsonElement selfElement = jsonDocument.RootElement.GetProperty(IanaLinkRelations.Self);
            
        Link? selfLink = JsonSerializer.Deserialize<Link>(selfElement.ToString(), options);

        if (selfLink is null)
        {
            throw new JsonException($"Unable to extract [{IanaLinkRelations.Self}] link from document.");
        }

        ClientResourceDocument<TResource> resourceDocument = new(selfLink, clientResource);
        
        jsonDocument.RootElement.ForEachLinkProperty((linkName, link) => resourceDocument.WithLink(linkName, link), options);
        
        return resourceDocument;
    }

    public override void Write(Utf8JsonWriter writer, ClientResourceDocument<TResource> value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}