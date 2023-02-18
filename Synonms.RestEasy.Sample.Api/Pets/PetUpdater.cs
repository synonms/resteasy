using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Sample.Api.Pets;

public class PetUpdater : IAggregateUpdater<Pet, PetResource>
{
    public Maybe<Fault> Update(Pet aggregateRoot, PetResource resource) =>
        aggregateRoot.Update(resource);
}