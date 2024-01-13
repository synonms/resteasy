using Synonms.RestEasy.WebApi.Domain;
using Synonms.Functional;

namespace Synonms.RestEasy.Sample.Api.Contracts;

public class ContractCreator : IAggregateCreator<Contract, ContractResource>
{
    public Task<Result<Contract>> CreateAsync(ContractResource resource, CancellationToken cancellationToken) =>
        Task.FromResult(Contract.Create(resource));
}