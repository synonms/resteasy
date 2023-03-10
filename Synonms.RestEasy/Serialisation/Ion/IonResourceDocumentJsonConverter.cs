using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonResourceDocumentJsonConverter<TAggregateRoot, TResource> : JsonConverter<ResourceDocument<TAggregateRoot, TResource>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    public override ResourceDocument<TAggregateRoot, TResource> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }

    public override void Write(Utf8JsonWriter writer, ResourceDocument<TAggregateRoot, TResource> value, JsonSerializerOptions options)
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