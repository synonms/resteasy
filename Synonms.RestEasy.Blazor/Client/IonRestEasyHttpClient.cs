﻿using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Synonms.RestEasy.Core.Serialisation;
using Synonms.RestEasy.Core.Serialisation.Ion;

namespace Synonms.RestEasy.Blazor.Client;

public class IonRestEasyHttpClient : RestEasyHttpClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
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

    public IonRestEasyHttpClient(IAccessTokenProvider tokenProvider, HttpClient httpClient)
        : base(httpClient, JsonSerializerOptions, client => RequestAuthToken(tokenProvider, client))
    {
    }
}