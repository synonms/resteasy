using System.Net;
using System.Text.Json;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Testing.Extensions;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Schema;
using Synonms.RestEasy.Core.Schema.Forms;
using Synonms.RestEasy.Testing.Assertions;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public static class EditFormTest<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    public static EditFormTestBuilder<TAggregateRoot> Create(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Action<Form, TAggregateRoot> validateFormAction) => 
        new(httpClient, collectionPath, jsonSerialiserOptions, persistAggregateFunc, validateFormAction);
}

public class EditFormTestBuilder<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    public EditFormTestBuilder(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Action<Form, TAggregateRoot> validateFormAction)
    {
        Arrange = new PreArrangeStep(httpClient, collectionPath, jsonSerialiserOptions, persistAggregateFunc, validateFormAction);
    }

    public PreArrangeStep Arrange { get; }
    
    public class PreArrangeStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;
        private readonly Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> _persistAggregateFunc;
        private readonly Action<Form, TAggregateRoot> _validateFormAction;

        public PreArrangeStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Action<Form, TAggregateRoot> validateFormAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _persistAggregateFunc = persistAggregateFunc;
            _validateFormAction = validateFormAction;
        }

        public PostArrangeStep WithoutAggregate() =>
            new(_httpClient, _collectionPath, _jsonSerialiserOptions, null, _validateFormAction);

        public PostArrangeStep WithAggregate(TAggregateRoot aggregateRoot, params object[]? prerequisiteEntities) =>
            WithAggregate(new ArrangeAggregateInfo<TAggregateRoot>(aggregateRoot, prerequisiteEntities));

        public PostArrangeStep WithAggregate(ArrangeAggregateInfo<TAggregateRoot> arrangeAggregateInfo)
        {
            TAggregateRoot persistedAggregateRoot = _persistAggregateFunc.Invoke(arrangeAggregateInfo).Result;

            return new PostArrangeStep(_httpClient, _collectionPath, _jsonSerialiserOptions, persistedAggregateRoot, _validateFormAction);
        }
    }
    
    public class PostArrangeStep
    {
        public PostArrangeStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, TAggregateRoot? aggregateRoot, Action<Form, TAggregateRoot> validateFormAction)
        {
            Act = new ActStep(httpClient, collectionPath, jsonSerialiserOptions, aggregateRoot, validateFormAction);
        }
        
        public ActStep Act { get; }
    }
    
    public class ActStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;
        private readonly TAggregateRoot? _aggregateRoot;
        private readonly Action<Form, TAggregateRoot> _validateFormAction;
        private EntityId<TAggregateRoot> _id = EntityId<TAggregateRoot>.New();

        public ActStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, TAggregateRoot? aggregateRoot, Action<Form, TAggregateRoot> validateFormAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _aggregateRoot = aggregateRoot;
            _validateFormAction = validateFormAction;

            if (aggregateRoot is not null)
            {
                _id = aggregateRoot.Id;
            }
        }

        public AssertStep Assert =>
            new(_httpClient, _collectionPath, _jsonSerialiserOptions, _id, _aggregateRoot, _validateFormAction);
        
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

        // public ActStep WithEntityTag(EntityVersion entityVersion)
        // {
        //     _httpClient.WithEntityTag(entityVersion);
        //     
        //     return this;
        // }
        
        public ActStep WithCorrelationId(Guid correlationId)
        {
            _httpClient.WithCorrelationId(correlationId);

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
        private readonly string _editFormPath;
        private readonly EntityId<TAggregateRoot> _id;
        private readonly TAggregateRoot? _aggregateRoot;
        private readonly Action<Form, TAggregateRoot> _validateFormAction;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;

        public AssertStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, EntityId<TAggregateRoot> id, TAggregateRoot? aggregateRoot, Action<Form, TAggregateRoot> validateFormAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _id = id;
            _aggregateRoot = aggregateRoot;
            _validateFormAction = validateFormAction;

            _editFormPath = collectionPath + "/" + id.Value + "/" + IanaLinkRelations.Forms.Edit;
        }

        public void SucceedsWith(HttpStatusCode httpStatusCode)
        {
            Assert.True(_aggregateRoot is not null, "Aggregate not arranged when testing success path - call Arrange.WithAggregate() in test body.");

            HttpResponseMessage httpResponseMessage = _httpClient.GetAsync(_editFormPath).Result;

            httpResponseMessage.AssertSuccess(httpStatusCode);

            string content = httpResponseMessage.Content.ReadAsStringAsync().Result;

            FormDocument? formDocument = JsonSerializer.Deserialize<FormDocument>(content, _jsonSerialiserOptions);

            if (formDocument is null)
            {
                Assert.Fail($"Failed to deserialise {nameof(FormDocument)} from response: {content}");
                return;
            }

            _validateFormAction.Invoke(formDocument.Form, _aggregateRoot);

            Dictionary<string, Link> expectedLinks = new()
            {
                [IanaLinkRelations.Self] = Link.SelfLink(TestUriConverter.ToAbsoluteUri(_editFormPath))
            };

            AssertThat.Links(formDocument.Links).Presents(expectedLinks);
        }
        
        public void FailsWith(HttpStatusCode httpStatusCode)
        {
            HttpResponseMessage httpResponseMessage = _httpClient.GetAsync(_editFormPath).Result;

            httpResponseMessage.AssertFailure(httpStatusCode);
        }
    }
}