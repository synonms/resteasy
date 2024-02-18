using Synonms.Functional;
using Synonms.RestEasy.Core.Domain;

namespace Synonms.RestEasy.Sample.Api.Employees;

public class EmployeeCreator : IAggregateCreator<Employee, EmployeeResource>
{
    public Task<Result<Employee>> CreateAsync(EmployeeResource resource, CancellationToken cancellationToken) =>
        Task.FromResult(Employee.Create(resource));
}