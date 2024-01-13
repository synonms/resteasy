using System.Reflection;
using System.Text.Json;
using Synonms.RestEasy.WebApi.Attributes;
using Synonms.RestEasy.WebApi.Schema.Resources;
using Microsoft.AspNetCore.Mvc.Testing;
using Synonms.RestEasy.Core.Domain;

namespace Synonms.RestEasy.Testing.Tests;

public abstract partial class AuthenticatedTestFixture<TWebApplicationFactory, TEntryPoint, TAggregateRoot, TResource> : IAsyncDisposable
    where TWebApplicationFactory : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    protected readonly TWebApplicationFactory WebApplicationFactory;
    protected readonly string MediaType;
    protected readonly JsonSerializerOptions? JsonSerialiserOptions;
    protected readonly int PageLimit;
    protected readonly string CollectionPath; 
    protected readonly string UserId;

    private readonly Queue<object> _persistedEntities = new();
    private readonly object _bolt = new();

    protected AuthenticatedTestFixture(TWebApplicationFactory webApplicationFactory, string mediaType, JsonSerializerOptions? jsonSerialiserOptions, int pageLimit, string userId)
    {
        WebApplicationFactory = webApplicationFactory;
        MediaType = mediaType;
        JsonSerialiserOptions = jsonSerialiserOptions;
        PageLimit = pageLimit;
        UserId = userId;
        
        RestEasyResourceAttribute? attribute = typeof(TAggregateRoot).GetCustomAttribute<RestEasyResourceAttribute>();

        CollectionPath = attribute?.CollectionPath ?? throw new Exception($"Unable to determine collection path from aggregate root of type '{typeof(TAggregateRoot).Name}' - check the [RestEasyResource] attribute is present.");
    }

    public abstract ValueTask DisposeAsync();

    protected abstract TAggregateRoot GenerateUniqueAggregate();

    protected abstract TResource GenerateInvalidResource(TAggregateRoot? existingAggregateRoot);

    protected abstract TResource GenerateValidResource(TAggregateRoot? existingAggregateRoot);

    protected abstract Task<TAggregateRoot> PersistAggregateAsync(ArrangeAggregateInfo<TAggregateRoot> arrangeAggregateInfo);

    protected abstract Task PersistPrerequisitesAsync(ArrangeEntitiesInfo arrangeEntitiesInfo);

    protected abstract Task RemoveAggregateAsync(EntityId<TAggregateRoot> id);

    protected abstract Task<TAggregateRoot?> RetrieveAggregateAsync(EntityId<TAggregateRoot> id);

    protected abstract void ValidateAggregate(TAggregateRoot aggregateRoot, TResource resource);
    
    protected abstract void ValidateResource(TAggregateRoot aggregateRoot, TResource resource);

    protected IEnumerable<object> DequeueEntities()
    {
        Queue<object> entitiesToDelete;
                
        lock (_bolt)
        {
            entitiesToDelete = new Queue<object>(_persistedEntities);
                
            _persistedEntities.Clear();
        }

        return entitiesToDelete;
    }

    protected void EnqueueEntities(IEnumerable<object> entities)
    {
        lock (_bolt)
        {
            foreach (object entity in entities)
            {
                _persistedEntities.Enqueue(entity);
            }
        }
    }
}