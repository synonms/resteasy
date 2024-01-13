using Synonms.RestEasy.WebApi.Domain;
using Synonms.Functional;

namespace Synonms.RestEasy.Sample.Api.Addresses;

public class AddressUpdater : IAggregateUpdater<Address, AddressResource>
{
    public Task<Maybe<Fault>> UpdateAsync(Address aggregateRoot, AddressResource resource, CancellationToken cancellationToken) =>
        Task.FromResult(aggregateRoot.Update(resource));
}