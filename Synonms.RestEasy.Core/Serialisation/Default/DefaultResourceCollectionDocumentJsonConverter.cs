using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Schema;
using Synonms.RestEasy.Core.Schema.Resources;
using Synonms.RestEasy.Core.Serialisation.Ion;

namespace Synonms.RestEasy.Core.Serialisation.Default;

public class DefaultResourceCollectionDocumentJsonConverter<TResource> : JsonConverter<ResourceCollectionDocument<TResource>>
    where TResource : Resource
{
    public override ResourceCollectionDocument<TResource> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out JsonDocument? doc))
        {
            throw new JsonException("Failed to parse JsonDocument");
        }

        using JsonDocument jsonDocument = doc;

        JsonElement valueArray = jsonDocument.RootElement.GetProperty(IonPropertyNames.Value);

        if (valueArray.ValueKind != JsonValueKind.Array)
        {
            throw new JsonException($"Expected '{IonPropertyNames.Value}' property to be an array.");
        }

        List<TResource> clientResources = new();

        foreach (JsonElement valueElement in valueArray.EnumerateArray())
        {
            TResource? clientResource = JsonSerializer.Deserialize<TResource>(valueElement.ToString(), options);

            if (clientResource is null)
            {
                throw new JsonException($"Unable to deserialise client resource type {nameof(TResource)}.");
            }

            clientResources.Add(clientResource);
        }

        string selfLinkJson = jsonDocument.RootElement.TryGetProperty(IanaLinkRelations.Self, out JsonElement selfElement) ? selfElement.ToString() : string.Empty;
            
        Link selfLink = JsonSerializer.Deserialize<Link>(selfLinkJson, options) ?? Link.EmptyLink();

        Pagination? pagination = JsonSerializer.Deserialize<Pagination>(jsonDocument.RootElement.ToString(), options);

        if (pagination is null)
        {
            throw new JsonException($"Unable to extract pagination from document.");
        }

        ResourceCollectionDocument<TResource> resourceDocument = new(selfLink, clientResources, pagination);

        jsonDocument.RootElement.ForEachIonLinkProperty((linkName, link) => resourceDocument.WithLink(linkName, link), options);

        return resourceDocument;
    }

    public override void Write(Utf8JsonWriter writer, ResourceCollectionDocument<TResource> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(IonPropertyNames.Value);
        JsonSerializer.Serialize(writer, value.Resources, options);

        if (value.Links.TryGetValue(IanaLinkRelations.Self, out Link? selfLink))
        {
            writer.WritePropertyName(IanaLinkRelations.Self);
            JsonSerializer.Serialize(writer, (object)selfLink, options);
        }

        JsonSerializer.Serialize(writer, value.Pagination, options);

        writer.WriteEndObject();
    }
}