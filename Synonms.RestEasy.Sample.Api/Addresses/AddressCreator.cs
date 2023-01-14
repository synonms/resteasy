using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Sample.Api.Addresses;

public class AddressCreator : IAggregateCreator<Address, AddressResource>
{
    public Result<Address> Create(AddressResource resource) =>
        Address.Create(resource);
}