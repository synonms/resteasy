using System.Text.Json;
using Synonms.RestEasy.SharedKernel.Serialisation;

namespace Synonms.RestEasy.Extensions;

public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions ConfigureForRestEasy(this JsonSerializerOptions jsonSerialiserOptions)
    {
        jsonSerialiserOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        jsonSerialiserOptions.Converters.Add(new DateOnlyJsonConverter());
        jsonSerialiserOptions.Converters.Add(new OptionalDateOnlyJsonConverter());
        jsonSerialiserOptions.Converters.Add(new TimeOnlyJsonConverter());
        jsonSerialiserOptions.Converters.Add(new OptionalTimeOnlyJsonConverter());

        return jsonSerialiserOptions;
    }
}