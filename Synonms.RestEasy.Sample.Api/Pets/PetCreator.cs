using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Sample.Api.Pets;

public class PetCreator : IAggregateCreator<Pet, PetResource>
{
    public Result<Pet> Create(PetResource resource) =>
        Pet.Create(resource);
}