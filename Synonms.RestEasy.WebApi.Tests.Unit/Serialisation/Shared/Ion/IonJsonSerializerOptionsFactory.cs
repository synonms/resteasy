using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Core.Serialisation;
using Synonms.RestEasy.WebApi.Serialisation.Ion;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Ion;

public static class IonJsonSerializerOptionsFactory
{
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