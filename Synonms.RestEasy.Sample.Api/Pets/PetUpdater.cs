using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Sample.Api.Pets;

public class PetUpdater : IAggregateUpdater<Pet, PetResource>
{
    public Task<Maybe<Fault>> UpdateAsync(Pet aggregateRoot, PetResource resource, CancellationToken cancellationToken) =>
        Task.FromResult(aggregateRoot.Update(resource));
}