using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Sample.Api.People;

public class PersonUpdater : IAggregateUpdater<Person, PersonResource>
{
    public Maybe<Fault> Update(Person aggregateRoot, PersonResource resource) =>
        aggregateRoot.Update(resource);
}