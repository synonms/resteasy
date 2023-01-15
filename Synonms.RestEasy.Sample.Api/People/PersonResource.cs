using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Sample.Api.Addresses;

namespace Synonms.RestEasy.Sample.Api.People;

public class PersonResource : Resource<Person>
{
    public PersonResource()
    {
    }
    
    public PersonResource(EntityId<Person> id, Link selfLink) 
        : base(id, selfLink)
    {
    }

    public string Forename { get; set; } = string.Empty;
    
    public string Surname { get; set; } = string.Empty;
    
    public DateOnly DateOfBirth { get; set; } = DateOnly.MinValue;
    
    public EntityId<Address> HomeAddressId { get; set; } = EntityId<Address>.Uninitialised;
}