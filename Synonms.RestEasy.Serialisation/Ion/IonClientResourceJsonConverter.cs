using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Client;
using Synonms.RestEasy.SharedKernel.Extensions;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonClientResourceJsonConverter<TResource> : JsonConverter<TResource>
    where TResource : ClientResource, new()
{
    public override TResource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out JsonDocument? doc))
        {
            throw new JsonException("Failed to parse JsonDocument");
        }

        using JsonDocument jsonDocument = doc;

        TResource resource = new();

        foreach (JsonProperty jsonProperty in jsonDocument.RootElement.EnumerateObject())
        {
            PropertyInfo? propertyInfo = typeof(TResource).GetProperty(jsonProperty.Name.ToPascalCase(), BindingFlags.Instance | BindingFlags.Public);

            if (propertyInfo is null || propertyInfo.CanWrite is false)
            {
                continue;
            }

            object? value = jsonProperty.Value.Deserialize(propertyInfo.PropertyType, options);

            if (value is not null && value.GetType().IsAssignableTo(propertyInfo.PropertyType))
            {
                propertyInfo.SetValue(resource, value);
            }
        }
            
        return resource;
    }

    public override void Write(Utf8JsonWriter writer, TResource value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("id", value.Id);

        foreach (PropertyInfo propertyInfo in typeof(TResource).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (propertyInfo.Name.Equals("id", StringComparison.OrdinalIgnoreCase)
                || propertyInfo.Name.Equals("SelfLink", StringComparison.OrdinalIgnoreCase)
                || propertyInfo.Name.Equals("Links", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            
            writer.WritePropertyName(propertyInfo.Name.ToCamelCase());
            JsonSerializer.Serialize(writer, propertyInfo.GetValue(value), options);
        }

        writer.WritePropertyName(IanaLinkRelations.Self);
        JsonSerializer.Serialize(writer, (object)value.SelfLink, options);

        foreach ((string linkName, Link link) in value.Links)
        {
            writer.WritePropertyName(linkName.ToCamelCase());
            JsonSerializer.Serialize(writer, link, options);
        }
            
        writer.WriteEndObject();
    }
}