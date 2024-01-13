using System.Text.Json;
using Synonms.RestEasy.Core.Serialisation;

namespace Synonms.RestEasy.WebApi.Startup;

public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions ConfigureForRestEasyFramework(this JsonSerializerOptions jsonSerialiserOptions)
    {
        jsonSerialiserOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        jsonSerialiserOptions.Converters.Add(new DateOnlyJsonConverter());
        jsonSerialiserOptions.Converters.Add(new OptionalDateOnlyJsonConverter());
        jsonSerialiserOptions.Converters.Add(new TimeOnlyJsonConverter());
        jsonSerialiserOptions.Converters.Add(new OptionalTimeOnlyJsonConverter());

        return jsonSerialiserOptions;
    }
}