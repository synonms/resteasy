using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Synonms.RestEasy.Hypermedia.Default;

public class JsonOutputFormatter : TextOutputFormatter
{
    public JsonOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("*/*"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));

        SupportedEncodings.Add(Encoding.UTF8);
    }
    
    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        JsonSerializerOptions jsonSerializerOptions = new ()
        {
            Converters = { 
//                new CustomJsonConverterFactory()
            }
        };

        string json = JsonSerializer.Serialize(context.Object, jsonSerializerOptions);

        await context.HttpContext.Response.WriteAsync(json, selectedEncoding);
    }
}