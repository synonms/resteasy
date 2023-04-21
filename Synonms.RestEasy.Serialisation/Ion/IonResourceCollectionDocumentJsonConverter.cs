﻿using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Serialisation.Ion.Constants;
using Synonms.RestEasy.Serialisation.Ion.Extensions;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonResourceCollectionDocumentJsonConverter<TResource> : JsonConverter<ResourceCollectionDocument<TResource>>
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

        JsonElement selfElement = jsonDocument.RootElement.GetProperty(IanaLinkRelations.Self);

        Link? selfLink = JsonSerializer.Deserialize<Link>(selfElement.ToString(), options);

        if (selfLink is null)
        {
            throw new JsonException($"Unable to extract [{IanaLinkRelations.Self}] link from document.");
        }

        Pagination? pagination = JsonSerializer.Deserialize<Pagination>(jsonDocument.RootElement.ToString(), options);

        if (pagination is null)
        {
            throw new JsonException($"Unable to extract pagination from document.");
        }

        ResourceCollectionDocument<TResource> resourceDocument = new(selfLink, clientResources, pagination);

        jsonDocument.RootElement.ForEachLinkProperty((linkName, link) => resourceDocument.WithLink(linkName, link), options);

        return resourceDocument;
    }

    public override void Write(Utf8JsonWriter writer, ResourceCollectionDocument<TResource> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(IonPropertyNames.Value);
        JsonSerializer.Serialize(writer, value.Resources, options);

        foreach ((string key, Link link) in value.Links)
        {
            writer.WritePropertyName(key);
            JsonSerializer.Serialize(writer, (object)link, options);
        }

        JsonSerializer.Serialize(writer, value.Pagination, options);

        writer.WriteEndObject();
    }
}