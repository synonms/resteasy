﻿using Synonms.RestEasy.Abstractions.Attributes;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Serialisation.Tests.Unit.Framework;

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