using System.Net;
using System.Text.Json;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Testing.Extensions;
using Synonms.RestEasy.Testing.Assertions;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Schema;
using Synonms.RestEasy.WebApi.Schema.Resources;
using Xunit;

namespace Synonms.RestEasy.Testing.Tests;

public static class GetAllTest<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public static GetAllTestBuilder<TAggregateRoot, TResource> Create(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, int pageLimit, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Action<TAggregateRoot, TResource> validateResourceAction) => 
        new(httpClient, collectionPath, jsonSerialiserOptions, pageLimit, persistAggregateFunc, validateResourceAction);
}

public class GetAllTestBuilder<TAggregateRoot, TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    public GetAllTestBuilder(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, int pageLimit, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Action<TAggregateRoot, TResource> validateResourceAction)
    {
        Arrange = new PreArrangeStep(httpClient, collectionPath, jsonSerialiserOptions, pageLimit, persistAggregateFunc, validateResourceAction);
    }

    public PreArrangeStep Arrange { get; }
    
    public class PreArrangeStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;
        private readonly int _pageLimit;
        private readonly Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> _persistAggregateFunc;
        private readonly Action<TAggregateRoot, TResource> _validateResourceAction;

        public PreArrangeStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, int pageLimit, Func<ArrangeAggregateInfo<TAggregateRoot>, Task<TAggregateRoot>> persistAggregateFunc, Action<TAggregateRoot, TResource> validateResourceAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _pageLimit = pageLimit;
            _persistAggregateFunc = persistAggregateFunc;
            _validateResourceAction = validateResourceAction;
        }

        public PostArrangeStep WithoutAggregates() =>
            new(_httpClient, _collectionPath, _jsonSerialiserOptions, _pageLimit, Enumerable.Empty<TAggregateRoot>(), _validateResourceAction);

        public PostArrangeStep WithAggregates(params TAggregateRoot[] aggregateRoots) =>
            WithAggregates(aggregateRoots.Select(x => new ArrangeAggregateInfo<TAggregateRoot>(x, null)).ToArray());

        public PostArrangeStep WithAggregates(params ArrangeAggregateInfo<TAggregateRoot>[] arrangeAggregateInfos)
        {
            List<TAggregateRoot> persistedAggregateRoots = arrangeAggregateInfos
                .Select(arrangeAggregateInfo => _persistAggregateFunc.Invoke(arrangeAggregateInfo).Result)
                .ToList();

            return new PostArrangeStep(_httpClient, _collectionPath, _jsonSerialiserOptions, _pageLimit, persistedAggregateRoots, _validateResourceAction);
        }
    }
    
    public class PostArrangeStep
    {
        public PostArrangeStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, int pageLimit, IEnumerable<TAggregateRoot> aggregateRoots, Action<TAggregateRoot, TResource> validateResourceAction)
        {
            Act = new ActStep(httpClient, collectionPath, jsonSerialiserOptions, pageLimit, aggregateRoots, validateResourceAction);
        }
        
        public ActStep Act { get; }
    }
    
    public class ActStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;
        private readonly int _pageLimit;
        private readonly IEnumerable<TAggregateRoot> _aggregateRoots;
        private readonly Action<TAggregateRoot, TResource> _validateResourceAction;
        private int _offset = 0;

        public ActStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, int pageLimit, IEnumerable<TAggregateRoot> aggregateRoots, Action<TAggregateRoot, TResource> validateResourceAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _pageLimit = pageLimit;
            _aggregateRoots = aggregateRoots;
            _validateResourceAction = validateResourceAction;
        }

        public AssertStep Assert => new (_httpClient, _collectionPath, _jsonSerialiserOptions, _pageLimit, _aggregateRoots, _offset, _validateResourceAction);

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

        public ActStep WithOffset(int offset)
        {
            _offset = offset;

            return this;
        }
    }
    
    public class AssertStep
    {
        private readonly HttpClient _httpClient;
        private readonly string _collectionPath;
        private readonly string _getAllPath;
        private readonly string _createFormPath;
        private readonly IEnumerable<TAggregateRoot> _aggregateRoots;
        private readonly int _offset;
        private readonly int _limit;
        private readonly Action<TAggregateRoot, TResource> _validateResourceAction;
        private readonly JsonSerializerOptions? _jsonSerialiserOptions;

        public AssertStep(HttpClient httpClient, string collectionPath, JsonSerializerOptions? jsonSerialiserOptions, int pageLimit, IEnumerable<TAggregateRoot> aggregateRoots, int offset, Action<TAggregateRoot, TResource> validateResourceAction)
        {
            _httpClient = httpClient;
            _collectionPath = collectionPath;
            _jsonSerialiserOptions = jsonSerialiserOptions;
            _aggregateRoots = aggregateRoots;
            _offset = offset;
            _limit = pageLimit;
            _validateResourceAction = validateResourceAction;

            _getAllPath = offset == 0 ? collectionPath : collectionPath + "?offset=" + offset;
            _createFormPath = collectionPath + "/" + IanaLinkRelations.Forms.Create;
        }

        public void SucceedsWith(HttpStatusCode httpStatusCode)
        {
            HttpResponseMessage httpResponseMessage = _httpClient.GetAsync(_getAllPath).Result;

            httpResponseMessage.AssertSuccess(httpStatusCode);
            
            string content = httpResponseMessage.Content.ReadAsStringAsync().Result;

            ResourceCollectionDocument<TResource>? resourceCollectionDocument = JsonSerializer.Deserialize<ResourceCollectionDocument<TResource>>(content, _jsonSerialiserOptions);

            if (resourceCollectionDocument is null)
            {
                Assert.Fail($"Failed to deserialise {nameof(ResourceCollectionDocument<TResource>)} from response: {content}");
                return;
            }

            int matchedResourceCount = 0;
            
            foreach (TAggregateRoot aggregateRoot in _aggregateRoots)
            {
                TResource? resource = resourceCollectionDocument.Resources.SingleOrDefault(x => x.Id.Equals(aggregateRoot.Id.Value));

                if (resource is null)
                {
                    if (_limit >= _aggregateRoots.Count())
                    {
                        // We expect to all aggregates to be present
                        Assert.True(resource is not null, $"Unable to find resource for entity Id=[{aggregateRoot.Id}] in document.");
                    }
                    
                    continue;
                }

                matchedResourceCount++;

                _validateResourceAction.Invoke(aggregateRoot, resource);
                
                // TODO: Test Resource Links
//                AssertThat.Links(resource.Links).Presents(/*TODO*/);
            }

            if (_limit > _aggregateRoots.Count())
            {
                Assert.Equal(_aggregateRoots.Count(), matchedResourceCount);
            }
            else
            {
                Assert.Equal(_limit, matchedResourceCount);
            }
            
            Dictionary<string, Link> expectedLinks = new()
            {
                [IanaLinkRelations.Self] = Link.SelfLink(TestUriConverter.ToAbsoluteUri(_getAllPath)),
                [IanaLinkRelations.Forms.Create] = Link.CreateFormLink(TestUriConverter.ToAbsoluteUri(_createFormPath))
            };

            AssertThat.Links(resourceCollectionDocument.Links).Presents(expectedLinks);
            
            // TODO: Test Pagination Links
//            AssertThat.LinksFromPagination(resourceCollectionDocument.Pagination).Presents(/*TODO*/);
        }
        
        public void FailsWith(HttpStatusCode httpStatusCode)
        {
            HttpResponseMessage httpResponseMessage = _httpClient.GetAsync(_getAllPath).Result;

            httpResponseMessage.AssertFailure(httpStatusCode);
        }
    }
}