using Synonms.RestEasy.Sample.Api.Addresses;
using Synonms.RestEasy.Sample.Api.Infrastructure;

namespace Synonms.RestEasy.Sample.Api.Tests.Integration.Infrastructure;

public class TestAddressesRepository : InMemoryRepository<Address>
{
    public TestAddressesRepository() : base(new List<Address>() { SeedData.Addresses[0] })
    {
    }
}