using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

public class TestAggregateRoot : AggregateRoot<TestAggregateRoot>
{
    public bool SomeBool { get; set; }
    
    public int SomeInt { get; set; }

    public string SomeString { get; set; } = string.Empty;
    
    public string? SomeOptionalString { get; set; }

    public TestAggregateMember SomeChild { get; set; } = new();
    
    public Guid SomeOtherId { get; set; }
}
