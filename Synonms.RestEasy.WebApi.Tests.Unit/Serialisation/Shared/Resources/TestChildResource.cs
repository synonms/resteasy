using System;
using Synonms.RestEasy.WebApi.Attributes;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Resources;

public class TestChildResource : ChildResource
{
    public TestChildResource()
    {
    }
    
    public TestChildResource(Guid id) 
        : base(id)
    {
    }

    [RestEasyRequired]
    public bool SomeBool { get; set; }
    
    [RestEasyRequired]
    [RestEasyMinValue(0)]
    [RestEasyMaxValue(100)]
    public int SomeInt { get; set; }

    [RestEasyRequired]
    [RestEasyMaxLength(100)]
    public string SomeString { get; set; } = string.Empty;
    
    [RestEasyMaxLength(100)]
    public string? SomeOptionalString { get; set; }
}