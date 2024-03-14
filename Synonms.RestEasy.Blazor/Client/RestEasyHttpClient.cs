﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.Blazor.Faults;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Infrastructure;
using Synonms.RestEasy.Core.Schema.Client;
using Synonms.RestEasy.Core.Schema.Forms;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.Blazor.Client;

public abstract class RestEasyHttpClient : IRestEasyHttpClient
{
    protected readonly HttpClient HttpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly Func<HttpClient, Task<Result<HttpClient>>> _authorizationFunc;

    protected RestEasyHttpClient(HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions, Func<HttpClient, Task<Result<HttpClient>>> authorizationFunc)
    {
        HttpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions;
        _authorizationFunc = authorizationFunc;
    }

    public virtual async Task<Result<FormDocument>> CreateFormAsync(string uri, CancellationToken cancellationToken) =>
        await _authorizationFunc.Invoke(HttpClient)
            .BindAsync(async authorisedHttpClient =>
            {
                HttpResponseMessage response = await authorisedHttpClient.GetAsync(uri, cancellationToken);

                if (response.IsSuccessStatusCode is false)
                {
                    return new ApiFault($"Received status code '{response.StatusCode}'.");
                }

                string json = await response.Content.ReadAsStringAsync(cancellationToken);
                FormDocument? formDocument = JsonSerializer.Deserialize<FormDocument>(json, _jsonSerializerOptions);

                return formDocument is null ? new ApiFault("Unable to deserialise response body.") : Result<FormDocument>.Success(formDocument);
            });

    public virtual async Task<Maybe<Fault>> DeleteAsync(string uri, CancellationToken cancellationToken) =>
        await _authorizationFunc.Invoke(HttpClient)
            .BindAsync(async authorisedHttpClient =>
            {
                HttpResponseMessage response = await authorisedHttpClient.DeleteAsync(uri, cancellationToken);

                if (response.IsSuccessStatusCode is false)
                {
                    return new ApiFault($"Received status code '{response.StatusCode}'.");
                }

                return Maybe<Fault>.None;
            });

    public virtual async Task<Result<FormDocument>> EditFormAsync(string uri, CancellationToken cancellationToken) =>
        await _authorizationFunc.Invoke(HttpClient)
            .BindAsync(async authorisedHttpClient =>
            {
                HttpResponseMessage response = await authorisedHttpClient.GetAsync(uri, cancellationToken);

                if (response.IsSuccessStatusCode is false)
                {
                    return new ApiFault($"Received status code '{response.StatusCode}'.");
                }

                string json = await response.Content.ReadAsStringAsync(cancellationToken);
                FormDocument? formDocument = JsonSerializer.Deserialize<FormDocument>(json, _jsonSerializerOptions);

                return formDocument is null ? new ApiFault("Unable to deserialise response body.") : Result<FormDocument>.Success(formDocument);
            });

    public virtual async Task<Result<ResourceCollectionDocument<TResource>>> GetAllAsync<TResource>(string uri, CancellationToken cancellationToken) where TResource : Resource =>
        await _authorizationFunc.Invoke(HttpClient)
            .BindAsync(async authorisedHttpClient =>
            {
                HttpResponseMessage response = await authorisedHttpClient.GetAsync(uri, cancellationToken);

                if (response.IsSuccessStatusCode is false)
                {
                    return new ApiFault($"Received status code '{response.StatusCode}'.");
                }

                string json = await response.Content.ReadAsStringAsync(cancellationToken);
                ResourceCollectionResponse<TResource>? body = JsonSerializer.Deserialize<ResourceCollectionResponse<TResource>>(json, _jsonSerializerOptions);

                if (body is null)
                {
                    return new ApiFault("Unable to deserialise response body.");
                }

                return body.Match(
                    errorCollectionDocument => new ApiFault(errorCollectionDocument.Errors),
                    Result<ResourceCollectionDocument<TResource>>.Success);
            });

    public virtual async Task<Result<ResourceDocument<TResource>>> GetByIdAsync<TResource>(string uri, CancellationToken cancellationToken) where TResource : Resource =>
        await _authorizationFunc.Invoke(HttpClient)
            .BindAsync(async authorisedHttpClient =>
            {
                HttpResponseMessage response = await authorisedHttpClient.GetAsync(uri, cancellationToken);

                if (response.IsSuccessStatusCode is false)
                {
                    return new ApiFault($"Received status code '{response.StatusCode}'.");
                }

                string json = await response.Content.ReadAsStringAsync(cancellationToken);
                ResourceResponse<TResource>? body = JsonSerializer.Deserialize<ResourceResponse<TResource>>(json, _jsonSerializerOptions);

                if (body is null)
                {
                    return new ApiFault("Unable to deserialise response body.");
                }

                return body.Match(
                    errorCollectionDocument => new ApiFault(errorCollectionDocument.Errors),
                    resourceDocument => Result<ResourceDocument<TResource>>.Success(resourceDocument));
            });

    public virtual async Task<Result<T?>> GetFromJsonAsync<T>(string uri, CancellationToken cancellationToken) =>
        await _authorizationFunc.Invoke(HttpClient)
            .BindAsync(async authorisedHttpClient =>
            {
                T? resource = await authorisedHttpClient.GetFromJsonAsync<T>(uri, _jsonSerializerOptions, cancellationToken);
                
                return Result<T?>.Success(resource);
            });

    public virtual async Task<Maybe<Fault>> PostAsync<TResource>(string uri, TResource resource, CancellationToken cancellationToken) where TResource : Resource =>
        await _authorizationFunc.Invoke(HttpClient)
            .BindAsync(async authorisedHttpClient =>
            {
                string json = JsonSerializer.Serialize(resource, _jsonSerializerOptions);
                StringContent stringContent = new(json, Encoding.UTF8, MediaTypes.Ion);

                HttpResponseMessage response = await authorisedHttpClient.PostAsync(uri, stringContent, cancellationToken);

                if (response.IsSuccessStatusCode is false)
                {
                    return new ApiFault($"Received status code '{response.StatusCode}'.");
                }

                return Maybe<Fault>.None;
            });

    public virtual async Task<Maybe<Fault>> PutAsync<TResource>(string uri, TResource resource, CancellationToken cancellationToken) where TResource : Resource =>
        await _authorizationFunc.Invoke(HttpClient)
            .BindAsync(async authorisedHttpClient =>
            {
                string json = JsonSerializer.Serialize(resource, _jsonSerializerOptions);
                StringContent stringContent = new(json, Encoding.UTF8, MediaTypes.Ion);

                HttpResponseMessage response = await authorisedHttpClient.PutAsync(uri, stringContent, cancellationToken);

                if (response.IsSuccessStatusCode is false)
                {
                    return new ApiFault($"Received status code '{response.StatusCode}'.");
                }

                return Maybe<Fault>.None;
            });

    public virtual async Task<Result<HttpResponseMessage>> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) =>
        await _authorizationFunc.Invoke(HttpClient)
            .BindAsync(async authorisedHttpClient =>
            {
                HttpResponseMessage httpResponseMessage = await authorisedHttpClient.SendAsync(httpRequestMessage, cancellationToken);
                
                return Result<HttpResponseMessage>.Success(httpResponseMessage);
            });
    
    protected static async Task<Result<HttpClient>> RequestAuthToken(IAccessTokenProvider accessTokenProvider, HttpClient httpClient)
    {
        AccessTokenResult requestToken = await accessTokenProvider.RequestAccessToken();
        
        if (requestToken.TryGetToken(out AccessToken? token) is false)
        {
            return new AuthorisationFault("Unable to obtain access token for RestEasy HTTP client.");
        }
        
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

        return httpClient;
    }
}