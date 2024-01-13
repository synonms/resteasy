using Synonms.RestEasy.Sample.Api.Employees;
using Synonms.RestEasy.Sample.Api.Infrastructure;

namespace Synonms.RestEasy.Sample.Api.Tests.Integration.Infrastructure;

public class TestEmployeesRepository : InMemoryRepository<Employee>
{
    public TestEmployeesRepository() : base(Enumerable.Empty<Employee>())
    {
    }
}