using Synonms.RestEasy.Sample.Api.Infrastructure;

namespace Synonms.RestEasy.Sample.Api.Contracts;

public class ContractsRepository : InMemoryRepository<Contract>
{
    public ContractsRepository() : base(SeedData.Contracts)
    {
    }
}