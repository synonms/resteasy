using System;
using System.Collections.Generic;
using System.Linq;
using Synonms.RestEasy.WebApi.Attributes;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Shared;

internal class TestDecoratedResource : Resource
{
    [RestEasyDescriptor("label", "placeholder", "description", "name")]
    public string PropertyWithDescriptor { get; set; } = string.Empty;
    
    [RestEasyDisabled]
    public bool PropertyWithDisabled { get; set; }
    
    [RestEasyHidden]
    public bool PropertyWithHidden { get; set; }
    
    [RestEasyImmutable]
    public bool PropertyWithImmutable { get; set; }
    
    [RestEasyMaxLength(11)]
    public string PropertyWithMaxLength { get; set; } = string.Empty;

    [RestEasyMaxSize(12)] 
    public int[] PropertyWithMaxSize { get; set; } = Array.Empty<int>();
    
    [RestEasyMaxValue(13)]
    public int PropertyWithMaxValue { get; set; }
    
    [RestEasyMinLength(1)]
    public string PropertyWithMinLength { get; set; } = string.Empty;

    [RestEasyMinSize(2)] 
    public int[] PropertyWithMinSize { get; set; } = Array.Empty<int>();
    
    [RestEasyMinValue(3)]
    public int PropertyWithMinValue { get; set; }
    
    [RestEasyOption(1, "one")]
    [RestEasyOption(2, "two")]
    public int PropertyWithOptions { get; set; }
    
    [RestEasyPattern("pattern")]
    public string PropertyWithPattern { get; set; } = string.Empty;
    
    [RestEasyRequired]
    public bool PropertyWithRequired { get; set; }
    
    [RestEasySecret]
    public bool PropertyWithSecret { get; set; }

    public string PropertyWithStringValue { get; set; } = "test";
    
    public int PropertyWithIntValue { get; set; } = 99;
    
    public bool PropertyWithBoolValue { get; set; } = true;
    
    [RestEasyPattern("pattern")]
    public IEnumerable<string> PropertyWithEForm { get; set; } = Enumerable.Empty<string>();
    
    public TestDecoratedChildResource PropertyWithChildResource { get; set; } = new();

    [RestEasyDescriptor("label", "placeholder", "description", "name")]
    [RestEasyDisabled]
    [RestEasyHidden]
    [RestEasyImmutable]
    [RestEasyMaxLength(100)]
    [RestEasyMaxSize(200)]
    [RestEasyMaxValue(300)]
    [RestEasyMinLength(10)]
    [RestEasyMinSize(20)]
    [RestEasyMinValue(30)]
    [RestEasyOption(1, "one")]
    [RestEasyOption(2, "two")]
    [RestEasyPattern("pattern")]
    [RestEasyRequired]
    [RestEasySecret]
    public int SimplePropertyWithAllAttributes { get; set; } = 123;

    [RestEasyDescriptor("label", "placeholder", "description", "name")]
    [RestEasyDisabled]
    [RestEasyHidden]
    [RestEasyImmutable]
    [RestEasyMaxLength(100)]
    [RestEasyMaxSize(200)]
    [RestEasyMaxValue(300)]
    [RestEasyMinLength(10)]
    [RestEasyMinSize(20)]
    [RestEasyMinValue(30)]
    [RestEasyOption(1, "one")]
    [RestEasyOption(2, "two")]
    [RestEasyPattern("pattern")]
    [RestEasyRequired]
    [RestEasySecret]
    public IEnumerable<string> EnumerablePropertyWithAllAttributes { get; set; } = new []{ "one", "two" };
}