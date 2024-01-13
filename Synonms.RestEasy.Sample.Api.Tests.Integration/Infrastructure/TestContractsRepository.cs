using Synonms.RestEasy.Sample.Api.Contracts;
using Synonms.RestEasy.Sample.Api.Infrastructure;

namespace Synonms.RestEasy.Sample.Api.Tests.Integration.Infrastructure;

public class TestContractsRepository : InMemoryRepository<Contract>
{
    public TestContractsRepository() : base(Enumerable.Empty<Contract>())
    {
    }
}