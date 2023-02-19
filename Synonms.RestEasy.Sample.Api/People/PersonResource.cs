using Synonms.RestEasy.Abstractions.Attributes;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Constants;
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

    [RestEasyRequired]
    [RestEasyMaxLength(Person.ForenameMaxLength)]
    public string Forename { get; set; } = string.Empty;
    
    [RestEasyRequired]
    [RestEasyMaxLength(Person.SurnameMaxLength)]
    public string Surname { get; set; } = string.Empty;

    [RestEasyRequired]
    [RestEasyPattern(RegularExpressions.DateOnly)]
    [RestEasyDescriptor(placeholder: Placeholders.DateOnly)]
    public DateOnly DateOfBirth { get; set; } = DateOnly.MinValue;

    [RestEasyLookup("Colour")]
    public string? FavouriteColour { get; set; }
    
    [RestEasyRequired]
    [RestEasyPattern(RegularExpressions.Guid)]
    [RestEasyDescriptor(placeholder: Placeholders.Guid)]
    public EntityId<Address> HomeAddressId { get; set; } = EntityId<Address>.Uninitialised;
}