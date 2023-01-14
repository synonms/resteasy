using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.SharedKernel.Extensions;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonResourceJsonConverter<TAggregateRoot, TResource> : JsonConverter<TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    public override TResource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }

    public override void Write(Utf8JsonWriter writer, TResource value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("id", value.Id.Value);

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