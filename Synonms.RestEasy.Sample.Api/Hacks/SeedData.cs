using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Sample.Api.Addresses;
using Synonms.RestEasy.Sample.Api.People;
using Synonms.RestEasy.Sample.Api.Pets;

namespace Synonms.RestEasy.Sample.Api.Hacks;

public static class SeedData
{
    public static readonly List<Person> People = new()
    {
        FunctionalHelper.FromResult(Person.Create(new PersonResource{ Forename = "Kendrick", Surname = "Lamar", DateOfBirth = new DateOnly(1984, 5, 5), HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000001") })).WithId(EntityId<Person>.Parse("00000000-0000-0000-0000-000000000001")),
        FunctionalHelper.FromResult(Person.Create(new PersonResource{ Forename = "Michael", Surname = "Archer", DateOfBirth = new DateOnly(1984, 6, 6), HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000002") })).WithId(EntityId<Person>.Parse("00000000-0000-0000-0000-000000000002")),
        FunctionalHelper.FromResult(Person.Create(new PersonResource{ Forename = "Jeff", Surname = "Buckley", DateOfBirth = new DateOnly(1984, 7, 7), HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000003") })).WithId(EntityId<Person>.Parse("00000000-0000-0000-0000-000000000003"))
    };

    public static readonly List<Address> Addresses = new()
    {
        FunctionalHelper.FromResult(Address.Create(new AddressResource { Line1 = "Some Street", Line2 = "Svartalfheim", PostCode = "SV1 1SS" })).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000001")),
        FunctionalHelper.FromResult(Address.Create(new AddressResource { Line1 = "Awful Avenue", Line2 = "Alfheim", PostCode = "AL2 2AA" })).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000002")),
        FunctionalHelper.FromResult(Address.Create(new AddressResource { Line1 = "Manky Mews", Line2 = "Midgard", PostCode = "MI3 3MM" })).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000003"))
    };

    public static readonly List<Pet> Pets = new()
    {
        FunctionalHelper.FromResult(Pet.Create(new PetResource{ OwnerId = EntityId<Person>.Parse("00000000-0000-0000-0000-000000000001"), Name = "Wriggles", DateOfBirth = new DateOnly(1984, 5, 5) })).WithId(EntityId<Pet>.Parse("00000000-0000-0000-0002-000000000001")),
        FunctionalHelper.FromResult(Pet.Create(new PetResource{ OwnerId = EntityId<Person>.Parse("00000000-0000-0000-0000-000000000001"), Name = "Cooking Fat", DateOfBirth = new DateOnly(1984, 5, 5) })).WithId(EntityId<Pet>.Parse("00000000-0000-0000-0002-000000000002")),
        FunctionalHelper.FromResult(Pet.Create(new PetResource{ OwnerId = EntityId<Person>.Parse("00000000-0000-0000-0000-000000000002"), Name = "Numpty", DateOfBirth = new DateOnly(1984, 6, 6) })).WithId(EntityId<Pet>.Parse("00000000-0000-0000-0002-000000000003")),
        FunctionalHelper.FromResult(Pet.Create(new PetResource{ OwnerId = EntityId<Person>.Parse("00000000-0000-0000-0000-000000000002"), Name = "Spot", DateOfBirth = new DateOnly(1984, 6, 6) })).WithId(EntityId<Pet>.Parse("00000000-0000-0000-0002-000000000004")),
        FunctionalHelper.FromResult(Pet.Create(new PetResource{ OwnerId = EntityId<Person>.Parse("00000000-0000-0000-0000-000000000003"), Name = "Woofy Woofpants", DateOfBirth = new DateOnly(1984, 7, 7) })).WithId(EntityId<Pet>.Parse("00000000-0000-0000-0002-000000000005")),
        FunctionalHelper.FromResult(Pet.Create(new PetResource{ OwnerId = EntityId<Person>.Parse("00000000-0000-0000-0000-000000000003"), Name = "Squirrel", DateOfBirth = new DateOnly(1984, 7, 7) })).WithId(EntityId<Pet>.Parse("00000000-0000-0000-0002-000000000006"))
    };
}