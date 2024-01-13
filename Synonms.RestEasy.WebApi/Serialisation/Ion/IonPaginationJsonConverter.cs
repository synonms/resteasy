using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Schema;

namespace Synonms.RestEasy.WebApi.Serialisation.Ion;

public class IonPaginationJsonConverter : JsonConverter<Pagination>
{
    public override Pagination? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out JsonDocument? doc))
        {
            throw new JsonException("Failed to parse JsonDocument");
        }

        using JsonDocument jsonDocument = doc;

        int offset = jsonDocument.RootElement.GetProperty(IonPropertyNames.Pagination.Offset).GetInt32();
        int limit = jsonDocument.RootElement.GetProperty(IonPropertyNames.Pagination.Limit).GetInt32();
        int size = jsonDocument.RootElement.GetProperty(IonPropertyNames.Pagination.Size).GetInt32();

        Link? firstPageLink = GetPageLink(IanaLinkRelations.Pagination.First, jsonDocument.RootElement);

        if (firstPageLink is null)
        {
            throw new JsonException($"Failed to deserialise [{IanaLinkRelations.Pagination.First}] link from document.");
        }

        Link? lastPageLink = GetPageLink(IanaLinkRelations.Pagination.Last, jsonDocument.RootElement);

        if (lastPageLink is null)
        {
            throw new JsonException($"Failed to deserialise [{IanaLinkRelations.Pagination.Last}] link from document.");
        }

        Link? previousPageLink = GetPageLink(IanaLinkRelations.Pagination.Previous, jsonDocument.RootElement);
        Link? nextPageLink = GetPageLink(IanaLinkRelations.Pagination.Next, jsonDocument.RootElement);

        return new Pagination(offset, limit, size, firstPageLink, lastPageLink)
        {
            Previous = previousPageLink,
            Next = nextPageLink
        };
    }

    public override void Write(Utf8JsonWriter writer, Pagination value, JsonSerializerOptions options)
    {
        writer.WriteNumber(PropertyNames.Pagination.Offset, value.Offset);
        writer.WriteNumber(PropertyNames.Pagination.Limit, value.Limit);
        writer.WriteNumber(PropertyNames.Pagination.Size, value.Size);

        writer.WritePropertyName(IanaLinkRelations.Pagination.First);
        JsonSerializer.Serialize(writer, (object)value.First, options);

        writer.WritePropertyName(IanaLinkRelations.Pagination.Last);
        JsonSerializer.Serialize(writer, (object)value.Last, options);

        if (value.Previous is not null)
        {
            writer.WritePropertyName(IanaLinkRelations.Pagination.Previous);
            JsonSerializer.Serialize(writer, (object)value.Previous, options);
        }

        if (value.Next is not null)
        {
            writer.WritePropertyName(IanaLinkRelations.Pagination.Next);
            JsonSerializer.Serialize(writer, (object)value.Next, options);
        }
    }
    
    private Link? GetPageLink(string relation, JsonElement jsonElement)
    {
        string? href = jsonElement.TryGetProperty(relation, out JsonElement linkElement) ? linkElement.GetProperty(IonPropertyNames.Links.Uri).GetString() : null;

        return string.IsNullOrWhiteSpace(href) ? null : Link.PageLink(new Uri(href, UriKind.RelativeOrAbsolute));
    }
}