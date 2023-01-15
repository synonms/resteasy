using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Extensions;
using Synonms.RestEasy.Serialisation.Ion;
using Synonms.RestEasy.SharedKernel.Serialisation;

namespace Synonms.RestEasy.Hypermedia.Ion;

public class IonInputFormatter : TextInputFormatter
{
    public IonInputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(MediaTypes.Ion));

        SupportedEncodings.Add(Encoding.UTF8);
    }

    protected override bool CanReadType(Type type) => 
        type.IsResource();
    
    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        JsonSerializerOptions jsonSerializerOptions = new ()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { 
                new DateOnlyJsonConverter(),
                new OptionalDateOnlyJsonConverter(),
                new TimeOnlyJsonConverter(),
                new OptionalTimeOnlyJsonConverter(),
                new IonCustomJsonConverterFactory(),
                new IonLinkJsonConverter()
            }
        };

        try
        {
            using TextReader streamReader = context.ReaderFactory(context.HttpContext.Request.Body, encoding);

            string body = await streamReader.ReadToEndAsync();

            object? resource = JsonSerializer.Deserialize(body, context.ModelType, jsonSerializerOptions);

            return await InputFormatterResult.SuccessAsync(resource);
        }
        catch(Exception exception)
        {
            string message = exception.Message;
            return await InputFormatterResult.FailureAsync();
        }
    }
}