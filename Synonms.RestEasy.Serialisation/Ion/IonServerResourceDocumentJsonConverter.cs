using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Server;
using Synonms.RestEasy.Serialisation.Ion.Constants;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonServerResourceDocumentJsonConverter<TAggregateRoot, TResource> : JsonConverter<ServerResourceDocument<TAggregateRoot, TResource>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : ServerResource<TAggregateRoot>
{
    public override ServerResourceDocument<TAggregateRoot, TResource> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }

    public override void Write(Utf8JsonWriter writer, ServerResourceDocument<TAggregateRoot, TResource> value, JsonSerializerOptions options)
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