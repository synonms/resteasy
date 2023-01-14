using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Sample.Api.Addresses;

public class AddressUpdater : IAggregateUpdater<Address, AddressResource>
{
    public Maybe<Fault> Update(Address aggregateRoot, AddressResource resource) =>
        aggregateRoot.Update(resource);
}