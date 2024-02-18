using Synonms.Functional;
using Synonms.RestEasy.Core.Domain;

namespace Synonms.RestEasy.Sample.Api.Employees;

public class EmployeeUpdater : IAggregateUpdater<Employee, EmployeeResource>
{
    public Task<Maybe<Fault>> UpdateAsync(Employee aggregateRoot, EmployeeResource resource, CancellationToken cancellationToken) =>
        Task.FromResult(aggregateRoot.Update(resource));
}