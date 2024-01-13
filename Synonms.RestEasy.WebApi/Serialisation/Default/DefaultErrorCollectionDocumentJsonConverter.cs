using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Schema;
using Synonms.RestEasy.WebApi.Schema.Errors;

namespace Synonms.RestEasy.WebApi.Serialisation.Default;

public class DefaultErrorCollectionDocumentJsonConverter : JsonConverter<ErrorCollectionDocument> 
{
    public override ErrorCollectionDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out JsonDocument? doc))
        {
            throw new JsonException("Failed to parse JsonDocument");
        }

        using JsonDocument jsonDocument = doc;

        JsonElement errorsArray = jsonDocument.RootElement.GetProperty(DefaultPropertyNames.Errors);

        if (errorsArray.ValueKind != JsonValueKind.Array)
        {
            throw new JsonException($"Expected [{DefaultPropertyNames.Value}] property to be an array.");
        }

        List<Error> errors = new();
            
        foreach (JsonElement errorElement in errorsArray.EnumerateArray())
        {
            Error? error = JsonSerializer.Deserialize<Error>(errorElement.ToString(), options);

            if (error is null)
            {
                throw new JsonException("Unable to deserialise error.");
            }

            errors.Add(error);
        }

        string selfLinkJson = jsonDocument.RootElement.TryGetProperty(IanaLinkRelations.Self, out JsonElement selfElement) ? selfElement.ToString() : string.Empty;
            
        Link selfLink = JsonSerializer.Deserialize<Link>(selfLinkJson, options) ?? Link.EmptyLink();

        ErrorCollectionDocument errorCollectionDocument = new(selfLink, errors);

        return errorCollectionDocument;
    }

    public override void Write(Utf8JsonWriter writer, ErrorCollectionDocument value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(DefaultPropertyNames.Errors);
        JsonSerializer.Serialize(writer, (object)value.Errors, options);

        if (value.Links.TryGetValue(IanaLinkRelations.Self, out Link? selfLink))
        {
            writer.WritePropertyName(IanaLinkRelations.Self);
            JsonSerializer.Serialize(writer, (object)selfLink, options);
        }

        writer.WriteEndObject();
    }
}