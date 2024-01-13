using Synonms.RestEasy.Sample.Api.Infrastructure;

namespace Synonms.RestEasy.Sample.Api.Employees;

public class EmployeesRepository : InMemoryRepository<Employee>
{
    public EmployeesRepository() : base(SeedData.Employees)
    {
    }
}