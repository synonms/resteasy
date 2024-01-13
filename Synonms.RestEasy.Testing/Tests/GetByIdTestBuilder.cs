using System.Net;
using System.Text.Json;
using Synonms.RestEasy.Testing.Extensions;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Testing.Assertions;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Schema;
using Synonms.RestEasy.WebApi.Schema.Resources;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public static class GetByIdTest<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public static GetByIdTestBuilder<TAggregateRoot, TResource> Create(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Action<TAggregateRoot, TResource> validateResourceAction) => 
        new(httpClient, collectionPath, jsonSerialiserOptions, persistAggregateFunc, validateResourceAction);
}

public class GetByIdTestBuilder<TAggregateRoot, TResource> 
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public GetByIdTestBuilder(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Action<TAggregateRoot, TResource> validateResourceAction)
    {
        Arrange = new PreArrangeStep(httpClient, collectionPath, jsonSerialiserOptions, persistAggregateFunc, validateResourceAction);
    }

    public PreArrangeStep Arrange { get; }
    
    public class PreArrangeStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;
        private readonly Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> _persistAggregateFunc;
        private readonly Action<TAggregateRoot, TResource> _validateResourceAction;

        public PreArrangeStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Action<TAggregateRoot, TResource> validateResourceAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _persistAggregateFunc = persistAggregateFunc;
            _validateResourceAction = validateResourceAction;
        }

        public PostArrangeStep WithoutAggregate() =>
            new(_httpClient, _collectionPath, _jsonSerialiserOptions, null, _validateResourceAction);

        public PostArrangeStep WithAggregate(TAggregateRoot aggregateRoot, params object[]? prerequisiteEntities) =>
            WithAggregate(new ArrangeAggregateInfo<TAggregateRoot>(aggregateRoot, prerequisiteEntities));
        
        public PostArrangeStep WithAggregate(ArrangeAggregateInfo<TAggregateRoot> arrangeAggregateInfo)
        {
            TAggregateRoot persistedAggregateRoot = _persistAggregateFunc.Invoke(arrangeAggregateInfo).Result;

            return new PostArrangeStep(_httpClient, _collectionPath, _jsonSerialiserOptions, persistedAggregateRoot, _validateResourceAction);
        }
    }
    
    public class PostArrangeStep
    {
        public PostArrangeStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, TAggregateRoot? aggregateRoot, Action<TAggregateRoot, TResource> validateResourceAction)
        {
            Act = new ActStep(httpClient, collectionPath, jsonSerialiserOptions, aggregateRoot, validateResourceAction);
        }
        
        public ActStep Act { get; }
    }
    
    public class ActStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;
        private readonly TAggregateRoot? _aggregateRoot;
        private readonly Action<TAggregateRoot, TResource> _validateResourceAction;
        private EntityId<TAggregateRoot> _id = EntityId<TAggregateRoot>.New();

        public ActStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, TAggregateRoot? aggregateRoot, Action<TAggregateRoot, TResource> validateResourceAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _aggregateRoot = aggregateRoot;
            _validateResourceAction = validateResourceAction;

            if (aggregateRoot is not null)
            {
                _id = aggregateRoot.Id;
            }
        }

        public AssertStep Assert =>
            new(_httpClient, _collectionPath, _jsonSerialiserOptions, _id, _aggregateRoot, _validateResourceAction);

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
    }
    
    public class AssertStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly string _getByIdPath;
        private readonly EntityId<TAggregateRoot> _id;
        private readonly TAggregateRoot? _aggregateRoot;
        private readonly Action<TAggregateRoot, TResource> _validateResourceAction;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;

        public AssertStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, EntityId<TAggregateRoot> id, TAggregateRoot? aggregateRoot, Action<TAggregateRoot, TResource> validateResourceAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _id = id;
            _aggregateRoot = aggregateRoot;
            _validateResourceAction = validateResourceAction;

            _getByIdPath = collectionPath + "/" + id.Value;
        }
        
        public void SucceedsWith(HttpStatusCode httpStatusCode)
        {
            Assert.True(_aggregateRoot is not null, "Entity not arranged when testing success path - call Arrange.WithAggregate() in test body.");
            
            HttpResponseMessage httpResponseMessage = _httpClient.GetAsync(_getByIdPath).Result;

            httpResponseMessage.AssertSuccess(httpStatusCode);

            string content = httpResponseMessage.Content.ReadAsStringAsync().Result;

            ResourceDocument<TResource>? resourceDocument = JsonSerializer.Deserialize<ResourceDocument<TResource>>(content, _jsonSerialiserOptions);

            if (resourceDocument is null)
            {
                Assert.Fail($"Failed to deserialise {nameof(ResourceDocument<TResource>)} from response: {content}");
                return;
            }

            _validateResourceAction.Invoke(_aggregateRoot!, resourceDocument.Resource);
            
            // TODO: Test Resource Links
//                AssertThat.Links(resource.Links).Presents(/*TODO*/);

            Dictionary<string, Link> expectedLinks = new()
            {
                [IanaLinkRelations.Self] = Link.SelfLink(TestUriConverter.ToAbsoluteUri(_getByIdPath))
            };

            AssertThat.Links(resourceDocument.Links).Presents(expectedLinks);
        }
        
        public void FailsWith(HttpStatusCode httpStatusCode)
        {
            HttpResponseMessage httpResponseMessage = _httpClient.GetAsync(_getByIdPath).Result;

            httpResponseMessage.AssertFailure(httpStatusCode);
        }
    }
}