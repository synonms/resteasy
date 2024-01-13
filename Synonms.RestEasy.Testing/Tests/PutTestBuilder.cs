using System.Net;
using System.Text;
using System.Text.Json;
using Synonms.RestEasy.Testing.Extensions;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.WebApi.Schema.Resources;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public static class PutTest<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public static PutTestBuilder<TAggregateRoot, TResource> Create(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, string mediaType, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, Action<TAggregateRoot, TResource> validateAggregateAction) => 
        new(httpClient, collectionPath, jsonSerialiserOptions, mediaType, persistAggregateFunc, retrieveAggregateFunc, validateAggregateAction);
}

public class PutTestBuilder<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public PutTestBuilder(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, string mediaType, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, Action<TAggregateRoot, TResource> validateAggregateAction)
    {
        Arrange = new ArrangeEntityStep(httpClient, collectionPath, jsonSerialiserOptions, mediaType, persistAggregateFunc, retrieveAggregateFunc, validateAggregateAction);
    }

    public ArrangeEntityStep Arrange { get; }
    
    public class ArrangeEntityStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;
        private readonly string _mediaType;
        private readonly Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> _persistAggregateFunc;
        private readonly Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> _retrieveAggregateFunc;
        private readonly Action<TAggregateRoot, TResource> _validateAggregateAction;

        public ArrangeEntityStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, string mediaType, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, Action<TAggregateRoot, TResource> validateAggregateAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _mediaType = mediaType;
            _persistAggregateFunc = persistAggregateFunc;
            _retrieveAggregateFunc = retrieveAggregateFunc;
            _validateAggregateAction = validateAggregateAction;
        }

        public ArrangeTemplateStep WithoutAggregate() =>
            new(_httpClient, _collectionPath, _jsonSerialiserOptions, _mediaType, _retrieveAggregateFunc, null, _validateAggregateAction);

        public ArrangeTemplateStep WithAggregate(TAggregateRoot aggregateRoot, params object[]? prerequisiteEntities) =>
            WithAggregate(new ArrangeAggregateInfo<TAggregateRoot>(aggregateRoot, prerequisiteEntities));

        public ArrangeTemplateStep WithAggregate(ArrangeAggregateInfo<TAggregateRoot> arrangeAggregateInfo)
        {
            TAggregateRoot persistedAggregateRoot = _persistAggregateFunc.Invoke(arrangeAggregateInfo).Result;

            return new ArrangeTemplateStep(_httpClient, _collectionPath, _jsonSerialiserOptions, _mediaType, _retrieveAggregateFunc, persistedAggregateRoot, _validateAggregateAction);
        }
    }
    
    public class ArrangeTemplateStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;
        private readonly string _mediaType;
        private readonly Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> _retrieveAggregateFunc;
        private readonly TAggregateRoot? _aggregateRoot;
        private readonly Action<TAggregateRoot, TResource> _validateAggregateAction;

        public ArrangeTemplateStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, string mediaType, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, TAggregateRoot? aggregateRoot, Action<TAggregateRoot, TResource> validateAggregateAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _mediaType = mediaType;
            _retrieveAggregateFunc = retrieveAggregateFunc;
            _aggregateRoot = aggregateRoot;
            _validateAggregateAction = validateAggregateAction;
        }

        public PostArrangeStep WithResource(Func<TAggregateRoot?, TResource> resourceFunc)
        {
            TResource resource = resourceFunc.Invoke(_aggregateRoot);
            
            return new PostArrangeStep(_httpClient, _collectionPath, _jsonSerialiserOptions, _mediaType, _retrieveAggregateFunc, resource, _aggregateRoot, _validateAggregateAction);
        }
    }
    
    public class PostArrangeStep
    {
        public PostArrangeStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, string mediaType, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, TResource resource, TAggregateRoot? entity, Action<TAggregateRoot, TResource> validateAggregateAction)
        {
            Act = new ActStep(httpClient, collectionPath, jsonSerialiserOptions, mediaType, retrieveAggregateFunc, resource, entity, validateAggregateAction);
        }
        
        public ActStep Act { get; }
    }
    
    public class ActStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;
        private readonly string _mediaType;
        private readonly Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> _retrieveAggregateFunc;
        private readonly TResource _resource;
        private readonly TAggregateRoot? _aggregateRoot;
        private readonly Action<TAggregateRoot, TResource> _validateAggregateAction;
        private EntityId<TAggregateRoot> _id = EntityId<TAggregateRoot>.New();
        private EntityTag? _entityTag = null;

        public ActStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, string mediaType, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, TResource resource, TAggregateRoot? aggregateRoot, Action<TAggregateRoot, TResource> validateAggregateAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _mediaType = mediaType;
            _retrieveAggregateFunc = retrieveAggregateFunc;
            _resource = resource;
            _aggregateRoot = aggregateRoot;
            _validateAggregateAction = validateAggregateAction;

            if (aggregateRoot is not null)
            {
                _id = aggregateRoot.Id;
                _entityTag = aggregateRoot.EntityTag;
            }
        }

        public AssertStep Assert =>
            new(_httpClient, _collectionPath, _jsonSerialiserOptions, _mediaType, _retrieveAggregateFunc, _id, _resource, _aggregateRoot, _entityTag, _validateAggregateAction);

        public ActStep WithAuthenticatedUser(string userId, params string[] permissions)
        {
            _httpClient.WithAuthenticatedUser(userId, permissions);
            
            return this;
        }

        public ActStep WithId(EntityId<TAggregateRoot> id)
        {
            _id = id;

            return this;
        }
        
        public ActStep WithoutVersion()
        {
            // TODO: Update HttpClient
            _entityTag = null;

            return this;
        }

        public ActStep WithEntityTag(EntityTag entityTag)
        {
            // TODO: Update HttpClient
            _entityTag = entityTag;

            return this;
        }
    }
    
    public class AssertStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly string _putPath;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;
        private readonly string _mediaType;
        private readonly Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> _retrieveAggregateFunc;
        private readonly EntityId<TAggregateRoot> _id;
        private readonly TResource _resource;
        private readonly TAggregateRoot? _aggregateRoot;
        private readonly EntityTag? _entityTag;
        private readonly Action<TAggregateRoot, TResource> _validateAggregateAction;

        public AssertStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, string mediaType, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, EntityId<TAggregateRoot> id, TResource resource, TAggregateRoot? aggregateRoot, EntityTag? entityTag, Action<TAggregateRoot, TResource> validateAggregateAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _mediaType = mediaType;
            _retrieveAggregateFunc = retrieveAggregateFunc;
            _id = id;
            _resource = resource;
            _aggregateRoot = aggregateRoot;
            _entityTag = entityTag;
            _validateAggregateAction = validateAggregateAction;

            _putPath = collectionPath + "/" + id.Value;
        }

        public void SucceedsWith(HttpStatusCode httpStatusCode)
        {
            Assert.True(_aggregateRoot is not null, "Aggregate not arranged when testing success path - call Arrange.WithAggregate() in test body.");
            
            string json = JsonSerializer.Serialize(_resource, _jsonSerialiserOptions);
            StringContent content = new(json, Encoding.UTF8, _mediaType);
            HttpResponseMessage httpResponseMessage = _httpClient.PutAsync(_putPath, content).Result;

            httpResponseMessage.AssertSuccess(httpStatusCode);

            TAggregateRoot? retrievedAggregateRoot = _retrieveAggregateFunc.Invoke(_id).Result;

            if (retrievedAggregateRoot is null)
            {
                Assert.Fail($"Unable to retrieve aggregate id [{_id}].");
                return;
            }

            _validateAggregateAction.Invoke(retrievedAggregateRoot, _resource);
        }
        
        public void FailsWith(HttpStatusCode httpStatusCode)
        {
            string json = JsonSerializer.Serialize(_resource, _jsonSerialiserOptions);
            StringContent content = new(json, Encoding.UTF8, _mediaType);
            HttpResponseMessage httpResponseMessage = _httpClient.PutAsync(_putPath, content).Result;

            httpResponseMessage.AssertFailure(httpStatusCode);
        }
    }
}