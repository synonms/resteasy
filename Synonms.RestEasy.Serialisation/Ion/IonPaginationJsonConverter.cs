using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Serialisation.Ion;

public class IonPaginationJsonConverter : JsonConverter<Pagination>
{
    public override Pagination? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Pagination value, JsonSerializerOptions options)
    {
        writer.WriteNumber(PropertyNames.Pagination.Offset, value.Offset);
        writer.WriteNumber(PropertyNames.Pagination.Limit, value.Limit);
        writer.WriteNumber(PropertyNames.Pagination.Size, value.Size);

        writer.WritePropertyName(IanaLinkRelations.Pagination.First);
        JsonSerializer.Serialize(writer, (object)value.First, options);

        writer.WritePropertyName(IanaLinkRelations.Pagination.Last);
        JsonSerializer.Serialize(writer, (object)value.Last, options);

        if (value.Previous is not null)
        {
            writer.WritePropertyName(IanaLinkRelations.Pagination.Previous);
            JsonSerializer.Serialize(writer, (object)value.Previous, options);
        }

        if (value.Next is not null)
        {
            writer.WritePropertyName(IanaLinkRelations.Pagination.Next);
            JsonSerializer.Serialize(writer, (object)value.Next, options);
        }
    }
}