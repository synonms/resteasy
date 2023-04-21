using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Serialisation.Ion.Constants;
using Synonms.RestEasy.Serialisation.Ion.Extensions;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonResourceDocumentJsonConverter<TResource> : JsonConverter<ResourceDocument<TResource>>
    where TResource : Resource
{
    public override ResourceDocument<TResource> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        ResourceDocument<TResource> resourceDocument = new(selfLink, clientResource);

        jsonDocument.RootElement.ForEachLinkProperty((linkName, link) => resourceDocument.WithLink(linkName, link), options);

        return resourceDocument;
    }

    public override void Write(Utf8JsonWriter writer, ResourceDocument<TResource> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(IonPropertyNames.Value);
        JsonSerializer.Serialize(writer, value.Resource, options);

        foreach ((string key, Link link) in value.Links)
        {
            writer.WritePropertyName(key);
            JsonSerializer.Serialize(writer, (object)link, options);
        }

        writer.WriteEndObject();
    }
}