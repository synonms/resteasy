using Synonms.RestEasy.WebApi.Pipeline.Tenants;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Shared;

// Needs to be public for NSubstitute to mock the Repository
public class TestTenant : RestEasyTenant
{
    public string Name { get; set; } = string.Empty;
}