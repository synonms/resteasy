using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Client;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Abstractions.Schema.Forms;
using Synonms.RestEasy.Blazor.Faults;
using Synonms.RestEasy.Serialisation.Ion;
using Synonms.RestEasy.SharedKernel.Serialisation;

namespace Synonms.RestEasy.Blazor.Client;

public class RestEasyHttpClient : IRestEasyHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
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
            new IonPaginationJsonConverter()
        }
    };

    public RestEasyHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<FormDocument>> CreateFormAsync(string uri, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(uri, cancellationToken);

        if (response.IsSuccessStatusCode is false)
        {
            return new ApiFault($"Received status code '{response.StatusCode}'.");
        }
        
        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        FormDocument? formDocument = JsonSerializer.Deserialize<FormDocument>(json, _jsonSerializerOptions);

        return formDocument is null ? new ApiFault("Unable to deserialise response body.") : formDocument;
    }

    public async Task<Maybe<Fault>> DeleteAsync(string uri, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync(uri, cancellationToken);
        
        if (response.IsSuccessStatusCode is false)
        {
            return new ApiFault($"Received status code '{response.StatusCode}'.");
        }
        
        return Maybe<Fault>.None;
    }

    public async Task<Result<FormDocument>> EditFormAsync(string uri, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(uri, cancellationToken);

        if (response.IsSuccessStatusCode is false)
        {
            return new ApiFault($"Received status code '{response.StatusCode}'.");
        }
        
        string json = await response.Content.ReadAsStringAsync(cancellationToken);
        FormDocument? formDocument = JsonSerializer.Deserialize<FormDocument>(json, _jsonSerializerOptions);

        return formDocument is null ? new ApiFault("Unable to deserialise response body.") : formDocument;
    }

    public async Task<Result<ResourceCollectionDocument<TResource>>> GetAllAsync<TResource>(string uri, CancellationToken cancellationToken) where TResource : Resource
    {
        HttpResponseMessage response = await _httpClient.GetAsync(uri, cancellationToken);

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
    }

    public async Task<Result<ResourceDocument<TResource>>> GetByIdAsync<TResource>(string uri, CancellationToken cancellationToken) where TResource : Resource
    {
        HttpResponseMessage response = await _httpClient.GetAsync(uri, cancellationToken);

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
    }

    public async Task<Maybe<Fault>> PostAsync<TResource>(string uri, TResource resource, CancellationToken cancellationToken) where TResource : Resource
    {
        string json = JsonSerializer.Serialize(resource, _jsonSerializerOptions);
        StringContent stringContent = new(json, Encoding.UTF8, MediaTypes.Ion);
        
        HttpResponseMessage response = await _httpClient.PostAsync(uri, stringContent, cancellationToken);

        if (response.IsSuccessStatusCode is false)
        {
            return new ApiFault($"Received status code '{response.StatusCode}'.");
        }
        
        return Maybe<Fault>.None;
    }

    public async Task<Maybe<Fault>> PutAsync<TResource>(string uri, TResource resource, CancellationToken cancellationToken) where TResource : Resource
    {
        string json = JsonSerializer.Serialize(resource, _jsonSerializerOptions);
        StringContent stringContent = new(json, Encoding.UTF8, MediaTypes.Ion);
        
        HttpResponseMessage response = await _httpClient.PutAsync(uri, stringContent, cancellationToken);

        if (response.IsSuccessStatusCode is false)
        {
            return new ApiFault($"Received status code '{response.StatusCode}'.");
        }
        
        return Maybe<Fault>.None;
    }
}