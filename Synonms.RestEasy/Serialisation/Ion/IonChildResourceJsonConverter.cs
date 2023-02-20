﻿using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.SharedKernel.Extensions;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonChildResourceJsonConverter<TAggregateMember, TChildResource> : JsonConverter<TChildResource>
    where TAggregateMember : AggregateMember<TAggregateMember>
    where TChildResource : ChildResource<TAggregateMember>, new()
{
    public override TChildResource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out JsonDocument? doc))
        {
            throw new JsonException("Failed to parse JsonDocument");
        }

        using JsonDocument jsonDocument = doc;

        TChildResource childResource = new();

        foreach (JsonProperty jsonProperty in jsonDocument.RootElement.EnumerateObject())
        {
            PropertyInfo? propertyInfo = typeof(TChildResource).GetProperty(jsonProperty.Name.ToPascalCase(), BindingFlags.Instance | BindingFlags.Public);

            if (propertyInfo is null || propertyInfo.CanWrite is false)
            {
                continue;
            }

            object? value = jsonProperty.Value.Deserialize(propertyInfo.PropertyType, options);

            if (value is not null && value.GetType().IsAssignableTo(propertyInfo.PropertyType))
            {
                propertyInfo.SetValue(childResource, value);
            }
        }
            
        return childResource;
    }

    public override void Write(Utf8JsonWriter writer, TChildResource value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("id", value.Id.Value);

        foreach (PropertyInfo propertyInfo in typeof(TChildResource).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (propertyInfo.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            
            writer.WritePropertyName(propertyInfo.Name.ToCamelCase());
            JsonSerializer.Serialize(writer, propertyInfo.GetValue(value), options);
        }
            
        writer.WriteEndObject();
    }
}