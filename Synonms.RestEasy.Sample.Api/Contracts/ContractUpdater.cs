using Synonms.RestEasy.WebApi.Domain;
using Synonms.Functional;

namespace Synonms.RestEasy.Sample.Api.Contracts;

public class ContractUpdater : IAggregateUpdater<Contract, ContractResource>
{
    public Task<Maybe<Fault>> UpdateAsync(Contract aggregateRoot, ContractResource resource, CancellationToken cancellationToken) =>
        Task.FromResult(aggregateRoot.Update(resource));
}