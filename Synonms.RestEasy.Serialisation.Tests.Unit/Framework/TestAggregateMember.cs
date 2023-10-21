using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

public class TestAggregateMember : AggregateMember<TestAggregateMember>
{
    public bool SomeBool { get; set; }
    
    public int SomeInt { get; set; }

    public string SomeString { get; set; } = string.Empty;
    
    public string? SomeOptionalString { get; set; }
}