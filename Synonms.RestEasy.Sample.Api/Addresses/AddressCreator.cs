using Synonms.RestEasy.WebApi.Domain;
using Synonms.Functional;

namespace Synonms.RestEasy.Sample.Api.Addresses;

public class AddressCreator : IAggregateCreator<Address, AddressResource>
{
    public Task<Result<Address>> CreateAsync(AddressResource resource, CancellationToken cancellationToken) =>
        Task.FromResult(Address.Create(resource));
}