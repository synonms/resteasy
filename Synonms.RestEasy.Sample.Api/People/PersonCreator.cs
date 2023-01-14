using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Sample.Api.People;

public class PersonCreator : IAggregateCreator<Person, PersonResource>
{
    public Result<Person> Create(PersonResource resource) =>
        Person.Create(resource);
}