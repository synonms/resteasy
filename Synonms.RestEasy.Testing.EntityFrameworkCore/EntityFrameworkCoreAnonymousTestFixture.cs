using System.Text.Json;
using Synonms.RestEasy.Testing.Tests;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.WebApi.Schema.Resources;
using Xunit;

namespace Synonms.RestEasy.Testing.EntityFrameworkCore;

public abstract class EntityFrameworkCoreAnonymousTestFixture<TWebApplicationFactory, TEntryPoint, TAggregateRoot, TResource, TDbContext> 
    : AnonymousTestFixture<TWebApplicationFactory, TEntryPoint, TAggregateRoot, TResource>
    where TWebApplicationFactory : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
    where TDbContext : DbContext
{
    protected EntityFrameworkCoreAnonymousTestFixture(TWebApplicationFactory webApplicationFactory, string mediaType, JsonSerializerOptions? jsonSerialiserOptions, int pageLimit) 
        : base(webApplicationFactory, mediaType, jsonSerialiserOptions, pageLimit)
    {
    }

    public override async ValueTask DisposeAsync() => 
        await WithDbContextAsync(async dbContext =>
        {
            List<object> entitiesToDelete = DequeueEntities().ToList();

            entitiesToDelete.Reverse();

            foreach (object entity in entitiesToDelete)
            {
                dbContext.Remove(entity);
            }

            await dbContext.SaveChangesAsync();
        });

    protected override Task<TAggregateRoot> PersistAggregateAsync(ArrangeAggregateInfo<TAggregateRoot> arrangeAggregateInfo)
    {
        IServiceScopeFactory scopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();

        using IServiceScope scope = scopeFactory.CreateScope();
        using TDbContext dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        List<object> addedEntities = new();

        object[] prerequisiteEntities = arrangeAggregateInfo.PrerequisiteEntities?.Entities ?? Array.Empty<object>();
        
        foreach (object entity in prerequisiteEntities)
        {
            EntityEntry entityEntry = dbContext.Add(entity);
            addedEntities.Add(entityEntry.Entity);
        }

        EntityEntry aggregateRootEntry = dbContext.Add(arrangeAggregateInfo.AggregateRoot);

        TAggregateRoot? persistedAggregateRoot = aggregateRootEntry.Entity as TAggregateRoot;

        if (persistedAggregateRoot is null)
        {
            Assert.Fail($"Failed to persist {nameof(TAggregateRoot)} aggregate.");
        }
        
        addedEntities.Add(aggregateRootEntry.Entity);

        dbContext.SaveChanges();

        EnqueueEntities(addedEntities);

        return Task.FromResult(persistedAggregateRoot!);
    }

    protected override Task PersistPrerequisitesAsync(ArrangeEntitiesInfo arrangeEntitiesInfo)
    {
        IServiceScopeFactory scopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();

        using IServiceScope scope = scopeFactory.CreateScope();
        using TDbContext dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        List<object> addedEntities = new();

        object[] prerequisiteEntities = arrangeEntitiesInfo.Entities ?? Array.Empty<object>();
        
        foreach (object entity in prerequisiteEntities)
        {
            EntityEntry entityEntry = dbContext.Add(entity);
            addedEntities.Add(entityEntry.Entity);
        }

        dbContext.SaveChanges();

        EnqueueEntities(addedEntities);
        
        return Task.CompletedTask;
    }

    protected override Task RemoveAggregateAsync(EntityId<TAggregateRoot> id)
    {
        IServiceScopeFactory scopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();

        using IServiceScope scope = scopeFactory.CreateScope();
        using TDbContext dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        TAggregateRoot? aggregateRoot = dbContext.Find<TAggregateRoot>(id);

        if (aggregateRoot is not null)
        {
            EntityEntry _ = dbContext.Remove(aggregateRoot);
        }
        
        return Task.CompletedTask;
    }

    protected override Task<TAggregateRoot?> RetrieveAggregateAsync(EntityId<TAggregateRoot> id)
    {
        IServiceScopeFactory scopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();

        using IServiceScope scope = scopeFactory.CreateScope();
        using TDbContext dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        TAggregateRoot? aggregateRoot = dbContext.Find<TAggregateRoot>(id);

        return Task.FromResult(aggregateRoot);
    }

    protected async Task WithDbContextAsync(Func<TDbContext, Task> func)
    {
        IServiceScopeFactory scopeFactory = WebApplicationFactory.Services.GetRequiredService<IServiceScopeFactory>();

        using IServiceScope scope = scopeFactory.CreateScope();
        await using TDbContext dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        await func.Invoke(dbContext);
    }
}