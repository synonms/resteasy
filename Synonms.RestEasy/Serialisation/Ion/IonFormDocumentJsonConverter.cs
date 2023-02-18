using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonFormDocumentJsonConverter : JsonConverter<FormDocument>
{
    public override FormDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }

    public override void Write(Utf8JsonWriter writer, FormDocument value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        writer.WriteString(IonPropertyNames.Links.Uri, value.Form.Target.Uri.OriginalString);
        writer.WriteString(IonPropertyNames.Links.Relation, value.Form.Target.Relation);
        writer.WriteString(IonPropertyNames.Links.Method, value.Form.Target.Method);

        writer.WritePropertyName(IonPropertyNames.Value);
        JsonSerializer.Serialize(writer, (object)value.Form.Fields, options);
        
        foreach ((string key, Link link) in value.Links)
        {
            writer.WritePropertyName(key);
            JsonSerializer.Serialize(writer, (object)link, options);
        }

        writer.WriteEndObject();
    }
}