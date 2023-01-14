using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonLinkJsonConverter : JsonConverter<Link>
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsAssignableTo(typeof(Link));

    public override Link Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }

    public override void Write(Utf8JsonWriter writer, Link value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString(IonPropertyNames.Links.Uri, value.Uri.OriginalString);
        writer.WriteString(IonPropertyNames.Links.Relation, value.Relation);
        writer.WriteString(IonPropertyNames.Links.Method, value.Method);

        if (value.Accepts?.Any() ?? false)
        {
            writer.WritePropertyName(IonPropertyNames.Links.Accepts);
            JsonSerializer.Serialize(writer, value.Accepts, options);
        }

        writer.WriteEndObject();
    }
}