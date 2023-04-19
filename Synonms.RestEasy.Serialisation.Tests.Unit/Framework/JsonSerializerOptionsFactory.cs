using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Serialisation.Ion;
using Synonms.RestEasy.SharedKernel.Serialisation;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

internal static class JsonSerializerOptionsFactory
{
    public static JsonSerializerOptions CreateDefault() =>
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new DateOnlyJsonConverter(),
                new OptionalDateOnlyJsonConverter(),
                new TimeOnlyJsonConverter(),
                new OptionalTimeOnlyJsonConverter()
            }
        };

    public static JsonSerializerOptions CreateIon() =>
        new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new DateOnlyJsonConverter(),
                new OptionalDateOnlyJsonConverter(),
                new TimeOnlyJsonConverter(),
                new OptionalTimeOnlyJsonConverter(),
                new IonCustomJsonConverterFactory(),
                new IonLinkJsonConverter(),
                new IonFormDocumentJsonConverter(),
                new IonFormFieldJsonConverter(),
                new IonPaginationJsonConverter()
            }
        };
}