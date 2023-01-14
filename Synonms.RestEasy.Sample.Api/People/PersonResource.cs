using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Sample.Api.Addresses;

namespace Synonms.RestEasy.Sample.Api.People;

public class PersonResource : Resource<Person>
{
    public PersonResource(string forename, string surname, DateOnly dateOfBirth, EntityId<Address> homeAddressId)
        : this(EntityId<Person>.Uninitialised, Link.EmptyLink(), forename, surname, dateOfBirth, homeAddressId)
    {
    }
    
    public PersonResource(EntityId<Person> id, Link selfLink, string forename, string surname, DateOnly dateOfBirth, EntityId<Address> homeAddressId) 
        : base(id, selfLink)
    {
        Forename = forename;
        Surname = surname;
        DateOfBirth = dateOfBirth;
        HomeAddressId = homeAddressId;
    }

    public string Forename { get; private set; }
    
    public string Surname { get; private set; }
    
    public DateOnly DateOfBirth { get; private set; }
    
    public EntityId<Address> HomeAddressId { get; private set; }
}