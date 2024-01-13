using Synonms.RestEasy.WebApi.MultiTenancy;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Shared;

// Needs to be public for NSubstitute to mock the Repository
public class TestTenant : Tenant
{
    public string Name { get; set; } = string.Empty;
}