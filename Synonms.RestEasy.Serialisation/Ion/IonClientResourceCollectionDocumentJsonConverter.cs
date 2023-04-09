using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Client;
using Synonms.RestEasy.Serialisation.Ion.Constants;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonClientResourceCollectionDocumentJsonConverter<TResource> : JsonConverter<ClientResourceCollectionDocument<TResource>>
    where TResource : ClientResource
{
    public override ClientResourceCollectionDocument<TResource> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        // TODO: Other links

        Pagination pagination = GetPagination(jsonDocument.RootElement);
            
        ClientResourceCollectionDocument<TResource> resourceDocument = new(selfLink, clientResources, pagination);

        return resourceDocument;
    }

    public override void Write(Utf8JsonWriter writer, ClientResourceCollectionDocument<TResource> value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
    
    private Pagination GetPagination(JsonElement jsonElement)
    {
        int offset = jsonElement.GetProperty(IonPropertyNames.Pagination.Offset).GetInt32();
        int limit = jsonElement.GetProperty(IonPropertyNames.Pagination.Limit).GetInt32();
        int size = jsonElement.GetProperty(IonPropertyNames.Pagination.Size).GetInt32();
            
        Link? firstPageLink = GetPageLink(IanaLinkRelations.Pagination.First, jsonElement);

        if (firstPageLink is null)
        {
            throw new JsonException($"Failed to deserialise [{IanaLinkRelations.Pagination.First}] link from document.");
        }

        Link? lastPageLink = GetPageLink(IanaLinkRelations.Pagination.Last, jsonElement);

        if (lastPageLink is null)
        {
            throw new JsonException($"Failed to deserialise [{IanaLinkRelations.Pagination.Last}] link from document.");
        }

        Link? previousPageLink = GetPageLink(IanaLinkRelations.Pagination.Previous, jsonElement);
        Link? nextPageLink = GetPageLink(IanaLinkRelations.Pagination.Next, jsonElement);

        return new Pagination(offset, limit, size, firstPageLink, lastPageLink)
        {
            Previous = previousPageLink,
            Next = nextPageLink
        };
    }

    private Link? GetPageLink(string relation, JsonElement jsonElement)
    {
        string? href = jsonElement.TryGetProperty(relation, out JsonElement linkElement) ? linkElement.GetProperty(IonPropertyNames.Links.Uri).GetString() : null;

        return string.IsNullOrWhiteSpace(href) ? null : Link.PageLink(new Uri(href, UriKind.RelativeOrAbsolute));
    }
}