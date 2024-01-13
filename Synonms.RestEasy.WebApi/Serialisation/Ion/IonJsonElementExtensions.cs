using System.Text.Json;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Schema;

namespace Synonms.RestEasy.WebApi.Serialisation.Ion;

public static class IonJsonElementExtensions
{
    public static void ForEachIonLinkProperty(this JsonElement jsonElement, Action<string, Link> action, JsonSerializerOptions options)
    {
        foreach (JsonProperty jsonProperty in jsonElement.EnumerateObject())
        {
            if (jsonProperty.NameEquals(IonPropertyNames.Value))
            {
                continue;
            }

            if (jsonProperty.NameEquals(IanaLinkRelations.Self))
            {
                continue;
            }

            if (jsonProperty.NameEquals(IanaLinkRelations.Pagination.First)
                || jsonProperty.NameEquals(IanaLinkRelations.Pagination.Last)
                || jsonProperty.NameEquals(IanaLinkRelations.Pagination.Next)
                || jsonProperty.NameEquals(IanaLinkRelations.Pagination.Previous))
            {
                continue;
            }

            if (jsonProperty.Value.ValueKind != JsonValueKind.Object)
            {
                continue;
            }
            
            if (jsonProperty.Value.TryGetProperty(IonPropertyNames.Links.Uri, out JsonElement _))
            {
                Link? link = JsonSerializer.Deserialize<Link>(jsonProperty.Value.ToString(), options);

                if (link is not null)
                {
                    action.Invoke(jsonProperty.Name, link);
                }
            }
        }
    }

    public static Uri GetIonUri(this JsonElement jsonElement)
    {
        string? href = jsonElement.TryGetProperty(IonPropertyNames.Links.Uri, out JsonElement hrefElement) ? hrefElement.GetString() : null;

        if (string.IsNullOrWhiteSpace(href))
        {
            throw new JsonException($"Unable to find [{IonPropertyNames.Links.Uri}] property.");
        }

        return new Uri(href, UriKind.RelativeOrAbsolute);
    }

    public static string GetIonLinkMethod(this JsonElement jsonElement) =>
        jsonElement.GetProperty(IonPropertyNames.Links.Method).GetString() ?? throw new JsonException($"Unable to extract [{IonPropertyNames.Links.Method}] property."); 

    public static string GetIonLinkRelation(this JsonElement jsonElement) =>
        jsonElement.GetProperty(IonPropertyNames.Links.Relation).GetString() ?? throw new JsonException($"Unable to extract [{IonPropertyNames.Links.Relation}] property."); 

    public static string[]? GetIonLinkAccepts(this JsonElement jsonElement) =>
        jsonElement.TryGetProperty(IonPropertyNames.Links.Accepts, out JsonElement methodElement) 
            ? JsonSerializer.Deserialize<string[]>(methodElement.ToString()) 
            : null;
}