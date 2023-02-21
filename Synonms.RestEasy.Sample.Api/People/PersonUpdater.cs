using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Sample.Api.People;

public class PersonUpdater : IAggregateUpdater<Person, PersonResource>
{
    public Task<Maybe<Fault>> UpdateAsync(Person aggregateRoot, PersonResource resource, CancellationToken cancellationToken) =>
        Task.FromResult(aggregateRoot.Update(resource));
}