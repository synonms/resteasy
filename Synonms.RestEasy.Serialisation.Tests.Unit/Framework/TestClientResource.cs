using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Client;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

public class TestClientResource: ClientResource
{
    public TestClientResource()
    {
    }
    
    public TestClientResource(Guid id, Link selfLink) 
        : base(id, selfLink)
    {
    }

    public bool SomeBool { get; set; }
    
    public int SomeInt { get; set; }

    public string SomeString { get; set; } = string.Empty;
    
    public string? SomeOptionalString { get; set; }
}