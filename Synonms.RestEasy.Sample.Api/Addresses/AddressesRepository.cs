using Synonms.RestEasy.Sample.Api.Infrastructure;

namespace Synonms.RestEasy.Sample.Api.Addresses;

public class AddressesRepository : InMemoryRepository<Address>
{
    public AddressesRepository() : base(SeedData.Addresses)
    {
    }
}