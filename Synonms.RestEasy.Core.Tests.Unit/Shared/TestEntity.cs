using Synonms.RestEasy.Core.Domain;

namespace Synonms.RestEasy.Core.Tests.Unit.Shared;

public class TestEntity : Entity<TestEntity>
{
    public TestEntity()
        : this(EntityId<TestEntity>.New(), string.Empty)
    {
    }

    public TestEntity(EntityId<TestEntity> id, string someProperty) 
    {
        Id = id;
        SomeProperty = someProperty;
    }
        
    public string SomeProperty { get; }
}