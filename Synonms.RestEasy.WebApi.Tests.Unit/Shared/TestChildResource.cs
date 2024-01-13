using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Shared;

internal class TestChildResource : Resource
{
    public string Property1 { get; set; } = string.Empty;

    public int Property2 { get; set; }
}