using Synonms.RestEasy.WebApi.Attributes;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Shared;

internal class TestDecoratedChildResource : Resource
{
    [RestEasyRequired]
    [RestEasyPattern("pattern")]
    public string Property1 { get; set; } = string.Empty;

    [RestEasyRequired]
    [RestEasyMaxValue(100)]
    public int Property2 { get; set; }
}