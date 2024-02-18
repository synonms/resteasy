using System.Net;
using System.Text;
using System.Text.Json;
using Synonms.RestEasy.Testing.Extensions;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Schema.Resources;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public static class PostTest<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public static PostTestBuilder<TAggregateRoot, TResource> Create(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, string mediaType, Func<ArrangeEntitiesInfo, Task> persistPrerequisitesFunc, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, Func<EntityId<TAggregateRoot>, Task> removeAggregateFunc, Action<TAggregateRoot, TResource> validateAggregateAction) => 
        new(httpClient, collectionPath, jsonSerialiserOptions, mediaType, persistPrerequisitesFunc, retrieveAggregateFunc, removeAggregateFunc, validateAggregateAction);
}

public class PostTestBuilder<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public PostTestBuilder(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, string mediaType, Func<ArrangeEntitiesInfo, Task> persistPrerequisitesFunc, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, Func<EntityId<TAggregateRoot>, Task> removeAggregateFunc, Action<TAggregateRoot, TResource> validateAggregateAction)
    {
        Arrange = new PreArrangeStep(httpClient, collectionPath, jsonSerialiserOptions, mediaType, persistPrerequisitesFunc, retrieveAggregateFunc, removeAggregateFunc, validateAggregateAction);
    }

    public PreArrangeStep Arrange { get; }
    
    public class PreArrangeStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;
        private readonly string _mediaType;
        private readonly Func<ArrangeEntitiesInfo, Task> _persistPrerequisitesFunc;
        private readonly Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> _retrieveAggregateFunc;
        private readonly Func<EntityId<TAggregateRoot>, Task> _removeAggregateFunc;
        private readonly Action<TAggregateRoot, TResource> _validateAggregateAction;

        public PreArrangeStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, string mediaType, Func<ArrangeEntitiesInfo, Task> persistPrerequisitesFunc, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, Func<EntityId<TAggregateRoot>, Task> removeAggregateFunc, Action<TAggregateRoot, TResource> validateAggregateAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _mediaType = mediaType;
            _persistPrerequisitesFunc = persistPrerequisitesFunc;
            _retrieveAggregateFunc = retrieveAggregateFunc;
            _removeAggregateFunc = removeAggregateFunc;
            _validateAggregateAction = validateAggregateAction;
        }
        
        public PostArrangeStep WithResource(Func<TAggregateRoot?, TResource> resourceFunc, params object[]? prerequisiteEntities) =>
            WithResource(resourceFunc, new ArrangeEntitiesInfo(prerequisiteEntities));
        
        public PostArrangeStep WithResource(Func<TAggregateRoot?, TResource> resourceFunc, ArrangeEntitiesInfo prerequisiteEntities)
        {
            TResource resource = resourceFunc.Invoke(null);
            
            _persistPrerequisitesFunc.Invoke(prerequisiteEntities).Wait();
            
            return new PostArrangeStep(_httpClient, _collectionPath, _jsonSerialiserOptions, _mediaType, _retrieveAggregateFunc, _removeAggregateFunc, resource, _validateAggregateAction);
        }
    }
    
    public class PostArrangeStep
    {
        public PostArrangeStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, string mediaType, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, Func<EntityId<TAggregateRoot>, Task> removeAggregateFunc, TResource resource, Action<TAggregateRoot, TResource> validateAggregateAction)
        {
            Act = new ActStep(httpClient, collectionPath, jsonSerialiserOptions, mediaType, retrieveAggregateFunc, removeAggregateFunc, resource, validateAggregateAction);
        }
        
        public ActStep Act { get; }
    }
    
    public class ActStep
    {
        private readonly HttpClient _httpClient;

        public ActStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, string mediaType, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, Func<EntityId<TAggregateRoot>, Task> removeAggregateFunc, TResource resource, Action<TAggregateRoot, TResource> validateAggregateAction)
        {
            _httpClient = httpClient;

            Assert = new AssertStep(httpClient, collectionPath, jsonSerialiserOptions, mediaType, retrieveAggregateFunc, removeAggregateFunc, resource, validateAggregateAction);
        }

        public AssertStep Assert { get; }

        public ActStep WithApiVersion(int apiVersion)
        {
            _httpClient.WithApiVersion(apiVersion);

            return this;
        }

        public ActStep WithAuthenticatedUser(string userId, params string[] permissions)
        {
            _httpClient.WithAuthenticatedUser(userId, permissions);
            
            return this;
        }
        
        public ActStep WithCorrelationId(Guid correlationId)
        {
            _httpClient.WithCorrelationId(correlationId);

            return this;
        }
    }
    
    public class AssertStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly string _mediaType;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;
        private readonly string _postPath;
        private readonly Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> _retrieveAggregateFunc;
        private readonly Func<EntityId<TAggregateRoot>, Task> _removeAggregateFunc;

        private readonly TResource _resource;
        private readonly Action<TAggregateRoot, TResource> _validateAggregateAction;

        public AssertStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, string mediaType, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, Func<EntityId<TAggregateRoot>, Task> removeAggregateFunc, TResource resource, Action<TAggregateRoot, TResource> validateAggregateAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _mediaType = mediaType;
            _retrieveAggregateFunc = retrieveAggregateFunc;
            _removeAggregateFunc = removeAggregateFunc;
            _resource = resource;
            _validateAggregateAction = validateAggregateAction;

            _postPath = collectionPath;
        }

        public void SucceedsWith(HttpStatusCode httpStatusCode)
        {
            string json = JsonSerializer.Serialize(_resource, _jsonSerialiserOptions);
            StringContent content = new(json, Encoding.UTF8, _mediaType);
            HttpResponseMessage httpResponseMessage = _httpClient.PostAsync(_postPath, content).Result;

            httpResponseMessage.AssertSuccess(httpStatusCode);

            Guid? newResourceId = httpResponseMessage.Headers.Location.ExtractTrailingId();

            if (newResourceId is null)
            {
                Assert.Fail($"Unable to determine new resource id from location [{httpResponseMessage.Headers.Location}].");
                return;
            }

            TAggregateRoot? retrievedAggregateRoot = _retrieveAggregateFunc.Invoke((EntityId<TAggregateRoot>)newResourceId).Result;

            if (retrievedAggregateRoot is null)
            {
                Assert.Fail($"Unable to retrieve aggregate id [{newResourceId}].");
                return;
            }

            _validateAggregateAction.Invoke(retrievedAggregateRoot, _resource);

            _removeAggregateFunc.Invoke((EntityId<TAggregateRoot>)newResourceId).Wait();
        }
        
        public void FailsWith(HttpStatusCode httpStatusCode)
        {
            string json = JsonSerializer.Serialize(_resource, _jsonSerialiserOptions);
            StringContent content = new(json, Encoding.UTF8, _mediaType);
            HttpResponseMessage httpResponseMessage = _httpClient.PostAsync(_postPath, content).Result;

            httpResponseMessage.AssertFailure(httpStatusCode);
        }
    }
}