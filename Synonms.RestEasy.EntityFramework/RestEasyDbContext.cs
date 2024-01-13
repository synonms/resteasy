using System.Reflection;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Domain.Events;
using Synonms.RestEasy.Core.Extensions;
using Synonms.RestEasy.Core.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.EntityFramework.ValueComparers;
using Synonms.RestEasy.EntityFramework.ValueConverters;

namespace Synonms.RestEasy.EntityFramework;

public abstract class RestEasyDbContext : DbContext
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    protected RestEasyDbContext(DbContextOptions<RestEasyDbContext> options, IDomainEventDispatcher domainEventDispatcher)
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }
    
    protected RestEasyDbContext(DbContextOptions options, IDomainEventDispatcher domainEventDispatcher)
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }
    
    public DbSet<Migration> Migrations { get; set; } = null!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        
        configurationBuilder
            .Properties<EntityTag>()
            .HaveConversion<EntityTagValueConverter>();

        configurationBuilder.Properties<DateOnly>()
            .HaveConversion<DateOnlyValueConverter, DateOnlyValueComparer>()
            .HaveColumnType("date");

        configurationBuilder.Properties<TimeOnly>()
            .HaveConversion<TimeOnlyValueConverter, TimeOnlyValueComparer>()
            .HaveColumnType("time");

        RegisterValueTypeValueConverters(configurationBuilder, EntityFrameworkCoreProject.Assembly);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(EntityFrameworkCoreProject.Assembly);

        RegisterEntityIdValueConverters(modelBuilder);
    }

    public override int SaveChanges() =>
        SaveChangesAsync(true).GetAwaiter().GetResult();
    
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new()) =>
        await DispatchDomainEventsAsync()
            .MatchAsync(
                fault => Task.FromResult(0),
                async () => await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken));

    protected static void RegisterValueTypeValueConverters(ModelConfigurationBuilder configurationBuilder, Assembly assembly) =>
        assembly.GetTypes()
            .Where(x => 
                !x.IsAbstract 
                && x.BaseType is not null 
                && x.BaseType.IsGenericType 
                && x.BaseType.GetGenericTypeDefinition() == typeof(ValueConverter<,>) 
                && x != typeof(EntityTagValueConverter)
                && x != typeof(EntityIdValueConverter<>)
                && x != typeof(DateOnlyValueConverter)
                && x != typeof(TimeOnlyValueConverter))
            .ToList()
            .ForEach(valueConverterType =>
            {
                Type valueObjectType = valueConverterType.BaseType!.GetGenericArguments().First();
                configurationBuilder.Properties(valueObjectType)
                    .HaveConversion(valueConverterType);
            });

    protected static void RegisterEntityIdValueConverters(ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType mutableEntityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (IMutableProperty mutableProperty in mutableEntityType.GetProperties())
            {
                if (mutableProperty.ClrType.IsEntityId())
                {
                    Type entityType = mutableProperty.ClrType.GetGenericArguments().Single();

                    Type converterType = typeof(EntityIdValueConverter<>).MakeGenericType(entityType);
                    
                    mutableProperty.SetValueConverter(converterType);
                }
            }
        }
    }
    
    private async Task<Maybe<Fault>> DispatchDomainEventsAsync()
    {
        IDomainEventProducer[] domainEventProducers = ChangeTracker.Entries<IDomainEventProducer>()
            .Select(entityEntry => entityEntry.Entity)
            .Where(x => x.DomainEvents.Any())
            .ToArray();

        foreach (IDomainEventProducer domainEventProducer in domainEventProducers)
        {
            while (domainEventProducer.DomainEvents.TryTake(out DomainEvent? domainEvent))
            {
                if (domainEvent is not null)
                {
                    Maybe<Fault> outcome = await _domainEventDispatcher.DispatchAsync(domainEvent);

                    if (outcome.IsSome)
                    {
                        return outcome;
                    }
                }
            }
        }
        
        return Maybe<Fault>.None;
    }
}