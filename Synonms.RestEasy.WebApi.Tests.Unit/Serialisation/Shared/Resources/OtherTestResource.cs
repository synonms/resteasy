using System;
using Synonms.RestEasy.Core.Attributes;
using Synonms.RestEasy.Core.Schema;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Serialisation.Shared.Resources;

public class OtherTestResource : Resource
{
    public OtherTestResource()
    {
    }
    
    public OtherTestResource(Guid id, Link selfLink) 
        : base(id, selfLink)
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