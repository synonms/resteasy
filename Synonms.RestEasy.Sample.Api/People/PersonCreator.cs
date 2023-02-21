using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Sample.Api.People;

public class PersonCreator : IAggregateCreator<Person, PersonResource>
{
    public Task<Result<Person>> CreateAsync(PersonResource resource, CancellationToken cancellationToken) =>
        Task.FromResult(Person.Create(resource));
}