using System.Net;
using Synonms.RestEasy.Testing.Extensions;
using Synonms.RestEasy.Core.Domain;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public static class DeleteTest<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    public static DeleteTestBuilder<TAggregateRoot> Create(HttpClient httpClient, string collectionPath, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc) => 
        new(httpClient, collectionPath, persistAggregateFunc, retrieveAggregateFunc);
}

public class DeleteTestBuilder<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    public DeleteTestBuilder(HttpClient httpClient, string collectionPath, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc)
    {
        Arrange = new PreArrangeStep(httpClient, collectionPath, persistAggregateFunc, retrieveAggregateFunc);
    }

    public PreArrangeStep Arrange { get; }
    
    public class PreArrangeStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> _persistAggregateFunc;
        private readonly Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> _retrieveAggregateFunc;

        public PreArrangeStep(HttpClient httpClient, string collectionPath, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _persistAggregateFunc = persistAggregateFunc;
            _retrieveAggregateFunc = retrieveAggregateFunc;
        }

        public PostArrangeStep WithoutAggregate() =>
            new(_httpClient, _collectionPath, _retrieveAggregateFunc);

        public PostArrangeStep WithAggregate(TAggregateRoot aggregateRoot, params object[]? prerequisiteEntities) => 
            WithAggregate(new ArrangeAggregateInfo<TAggregateRoot>(aggregateRoot, prerequisiteEntities));

        public PostArrangeStep WithAggregate(ArrangeAggregateInfo<TAggregateRoot> arrangeAggregateInfo)
        {
            TAggregateRoot persistedAggregateRoot = _persistAggregateFunc.Invoke(arrangeAggregateInfo).Result;

            return new PostArrangeStep(_httpClient, _collectionPath, _retrieveAggregateFunc, persistedAggregateRoot);
        }
    }
    
    public class PostArrangeStep
    {
        public PostArrangeStep(HttpClient httpClient, string collectionPath, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, TAggregateRoot? aggregateRoot = null)
        {
            Act = new ActStep(httpClient, collectionPath, retrieveAggregateFunc, aggregateRoot);
        }
        
        public ActStep Act { get; }
    }
    
    public class ActStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> _retrieveAggregateFunc;
        private EntityId<TAggregateRoot> _id = EntityId<TAggregateRoot>.New();
        private EntityTag? _entityTag = null;

        public ActStep(HttpClient httpClient, string collectionPath, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, TAggregateRoot? aggregateRoot)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _retrieveAggregateFunc = retrieveAggregateFunc;

            if (aggregateRoot is not null)
            {
                _id = aggregateRoot.Id;
                _entityTag = aggregateRoot.EntityTag;
            }
        }

        public AssertStep Assert =>
            new(_httpClient, _collectionPath, _retrieveAggregateFunc, _id, _entityTag);

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
        private readonly string _deletePath;
        private readonly Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> _retrieveAggregateFunc;
        private readonly EntityId<TAggregateRoot> _id;
        private readonly EntityTag? _entityTag;

        public AssertStep(HttpClient httpClient, string collectionPath, Func<EntityId<TAggregateRoot>, Task<TAggregateRoot?>> retrieveAggregateFunc, EntityId<TAggregateRoot> id, EntityTag? entityTag)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _retrieveAggregateFunc = retrieveAggregateFunc;
            _id = id;
            _entityTag = entityTag;

            _deletePath = collectionPath + "/" + _id.Value;
        }
        
        public void SucceedsWith(HttpStatusCode httpStatusCode)
        {
            HttpResponseMessage httpResponseMessage = _httpClient.DeleteAsync(_deletePath).Result;

            httpResponseMessage.AssertSuccess(httpStatusCode);

            TAggregateRoot? retrievedAggregateRoot = _retrieveAggregateFunc.Invoke(_id).Result;

            Assert.Null(retrievedAggregateRoot);
        }
        
        public void FailsWith(HttpStatusCode httpStatusCode)
        {
            HttpResponseMessage httpResponseMessage = _httpClient.DeleteAsync(_deletePath).Result;

            httpResponseMessage.AssertFailure(httpStatusCode);
        }
    }
}