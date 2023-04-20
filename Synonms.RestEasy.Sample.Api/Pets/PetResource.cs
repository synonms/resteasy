using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Sample.Api.People;

namespace Synonms.RestEasy.Sample.Api.Pets;

public class PetResource : Resource
{
    public PetResource()
    {
    }
    
    public PetResource(Guid id, Link selfLink) 
        : base(id, selfLink)
    {
    }

    public EntityId<Person> OwnerId { get; set; } = EntityId<Person>.Uninitialised;

    public string Name { get; set; } = string.Empty;
    
    public DateOnly DateOfBirth { get; set; } = DateOnly.MinValue;
}