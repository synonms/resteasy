﻿using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.RestEasy.Core.Serialisation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Serialisation.Ion;

namespace Synonms.RestEasy.WebApi.Hypermedia.Ion;

public class IonOutputFormatter : TextOutputFormatter
{
    public IonOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(MediaTypes.Ion));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(MediaTypes.AspNetCoreError));

        SupportedEncodings.Add(Encoding.UTF8);
    }

    protected override bool CanWriteType(Type? type) => 
        true;

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
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
                new IonLinkJsonConverter(),
                new IonFormDocumentJsonConverter(),
                new IonFormFieldJsonConverter(),
                new IonPaginationJsonConverter(),
                new IonErrorCollectionDocumentJsonConverter(),
                new IonErrorJsonConverter()
            }
        };

        string json = JsonSerializer.Serialize(context.Object, jsonSerializerOptions);

        await context.HttpContext.Response.WriteAsync(json, selectedEncoding);
    }
}