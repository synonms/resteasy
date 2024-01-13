using Synonms.RestEasy.WebApi.Domain;
using Synonms.Functional;

namespace Synonms.RestEasy.Sample.Api.Employees;

public class EmployeeCreator : IAggregateCreator<Employee, EmployeeResource>
{
    public Task<Result<Employee>> CreateAsync(EmployeeResource resource, CancellationToken cancellationToken) =>
        Task.FromResult(Employee.Create(resource));
}