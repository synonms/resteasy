using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Domain.ValueObjects;
using Synonms.RestEasy.Core.Tests.Unit.Shared;
using Xunit;

namespace Synonms.RestEasy.Core.Tests.Unit.Domain;

public class AggregateRootTests
{
    [Fact]
    public void Construction_Default_NewIdAndVersionIsGenerated()
    {
        TestAggregateRoot aggregateRoot = new();
        
        Assert.False(aggregateRoot.Id.IsEmpty);
        Assert.NotEqual(Guid.Empty, aggregateRoot.Id.Value);
        Assert.False(aggregateRoot.EntityTag.IsEmpty);
        Assert.NotEqual(Guid.Empty, aggregateRoot.EntityTag.Value);
    }
    
    [Fact]
    public void Construction_WithId_SetsIdAndNewVersionIsGenerated()
    {
        EntityId<TestAggregateRoot> id = EntityId<TestAggregateRoot>.New();
        TestAggregateRoot aggregateRoot = new(id);
        
        Assert.Equal(id, aggregateRoot.Id);
        Assert.False(aggregateRoot.EntityTag.IsEmpty);
        Assert.NotEqual(Guid.Empty, aggregateRoot.EntityTag.Value);
    }

    [Fact]
    public void UpdateProperty_DifferentValue_RevisesUpdatedAtAndVersion()
    {
        const string originalValue = "original";
        const string updatedValue = "updated";
        
        TestAggregateRoot aggregateRoot = new()
        {
            Name = Moniker.Convert(originalValue)
        };

        EntityTag originalEntityTag = aggregateRoot.EntityTag;
        DateTime originalCreatedAt = aggregateRoot.CreatedAt;
        DateTime? originalUpdatedAt = aggregateRoot.UpdatedAt;
        
        aggregateRoot.Update(updatedValue);

        Assert.Equal(updatedValue, aggregateRoot.Name);
        Assert.NotEqual(originalEntityTag, aggregateRoot.EntityTag);
        Assert.Equal(originalCreatedAt, aggregateRoot.CreatedAt);
        Assert.NotNull(aggregateRoot.UpdatedAt);
    }
    
    [Fact]
    public void UpdateProperty_SameValue_IsNoOp()
    {
        const string value = "original";
        
        TestAggregateRoot aggregateRoot = new()
        {
            Name = Moniker.Convert(value)
        };

        EntityTag originalEntityTag = aggregateRoot.EntityTag;
        DateTime originalCreatedAt = aggregateRoot.CreatedAt;
        DateTime? originalUpdatedAt = aggregateRoot.UpdatedAt;
        
        aggregateRoot.Update(value);

        Assert.Equal(value, aggregateRoot.Name);
        Assert.Equal(originalEntityTag, aggregateRoot.EntityTag);
        Assert.Equal(originalCreatedAt, aggregateRoot.CreatedAt);
        Assert.Null(aggregateRoot.UpdatedAt);
    }
}