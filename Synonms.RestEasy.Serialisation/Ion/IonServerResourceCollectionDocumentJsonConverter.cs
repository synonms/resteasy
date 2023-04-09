using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Server;
using Synonms.RestEasy.Serialisation.Ion.Constants;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonServerResourceCollectionDocumentJsonConverter<TAggregateRoot, TResource> : JsonConverter<ServerResourceCollectionDocument<TAggregateRoot, TResource>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : ServerResource<TAggregateRoot>
{
    public override ServerResourceCollectionDocument<TAggregateRoot, TResource> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }

    public override void Write(Utf8JsonWriter writer, ServerResourceCollectionDocument<TAggregateRoot, TResource> value, JsonSerializerOptions options)
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