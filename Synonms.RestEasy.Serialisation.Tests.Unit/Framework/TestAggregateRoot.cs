using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

public class TestAggregateRoot : AggregateRoot<TestAggregateRoot>
{
    public string SomeBool { get; set; }
    
    public string SomeInt { get; set; }

    public string SomeString { get; set; }
    
    public string? SomeOptionalString { get; set; }
}
