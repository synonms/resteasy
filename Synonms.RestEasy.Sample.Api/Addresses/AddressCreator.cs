using Synonms.Functional;
using Synonms.RestEasy.Core.Domain;

namespace Synonms.RestEasy.Sample.Api.Addresses;

public class AddressCreator : IAggregateCreator<Address, AddressResource>
{
    public Task<Result<Address>> CreateAsync(AddressResource resource, CancellationToken cancellationToken) =>
        Task.FromResult(Address.Create(resource));
}