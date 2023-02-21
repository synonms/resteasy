using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Sample.Api.Pets;

public class PetCreator : IAggregateCreator<Pet, PetResource>
{
    public Task<Result<Pet>> CreateAsync(PetResource resource, CancellationToken cancellationToken) =>
        Task.FromResult(Pet.Create(resource));
}