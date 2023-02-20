using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Sample.Api.Addresses;
using Synonms.RestEasy.Sample.Api.People;
using Synonms.RestEasy.Sample.Api.Pets;

namespace Synonms.RestEasy.Sample.Api.Hacks;

public static class SeedData
{
    public static readonly List<Person> People = new()
    {
        FunctionalHelper.FromResult(
            Person.Create(
                new PersonResource
                {
                    Forename = "Kendrick", 
                    Surname = "Lamar", 
                    DateOfBirth = new DateOnly(1994, 5, 5), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000001"),
                    GreatestAchievement = new PersonalAchievementResource { Description = "To Pimp A Butterfly", DateOfAchievement = new DateOnly(2015, 3, 16) },
                    Achievements = new []
                    {
                        new PersonalAchievementResource { Description = "Section.80", DateOfAchievement = new DateOnly(2011, 7, 2) },
                        new PersonalAchievementResource { Description = "Good Kid, M.A.A.D City", DateOfAchievement = new DateOnly(2012, 10, 22) },
                        new PersonalAchievementResource { Description = "DAMN", DateOfAchievement = new DateOnly(2017, 4, 14) },
                        new PersonalAchievementResource { Description = "Mr Morale & the Big Steppers", DateOfAchievement = new DateOnly(2022, 5, 13) },
                    }
                }))
            .WithId(EntityId<Person>.Parse("00000000-0000-0000-0000-000000000001")),
        FunctionalHelper.FromResult(
            Person.Create(
                new PersonResource
                {
                    Forename = "Michael", 
                    Surname = "Archer", 
                    DateOfBirth = new DateOnly(1984, 6, 6), 
                    FavouriteColour = "Brown", 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000002"),
                    GreatestAchievement = new PersonalAchievementResource { Description = "Brown Sugar", DateOfAchievement = new DateOnly(1995, 1, 1) }                
                }))
            .WithId(EntityId<Person>.Parse("00000000-0000-0000-0000-000000000002")),
        FunctionalHelper.FromResult(
            Person.Create(
                new PersonResource{ 
                    Forename = "Harry",
                    Surname = "Kane", 
                    DateOfBirth = new DateOnly(1994, 7, 7), 
                    FavouriteColour = "White", 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000003"),
                    GreatestAchievement = new PersonalAchievementResource { Description = "267 goals for Spurs", DateOfAchievement = new DateOnly(2023, 2, 5) }
                }))
            .WithId(EntityId<Person>.Parse("00000000-0000-0000-0000-000000000003")),
        FunctionalHelper.FromResult(
            Person.Create(
                new PersonResource{ 
                    Forename = "Heung-min",
                    Surname = "Son", 
                    DateOfBirth = new DateOnly(1994, 8, 8), 
                    FavouriteColour = "White", 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000003"),
                    GreatestAchievement = new PersonalAchievementResource { Description = "Joint Golden Boot winner", DateOfAchievement = new DateOnly(2022, 5, 22) } 
                }))
            .WithId(EntityId<Person>.Parse("00000000-0000-0000-0000-000000000004")),
        FunctionalHelper.FromResult(
            Person.Create(
                new PersonResource{ 
                    Forename = "Jeff",
                    Surname = "Buckley", 
                    DateOfBirth = new DateOnly(1994, 7, 7), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000004"),
                    GreatestAchievement = new PersonalAchievementResource { Description = "Grace", DateOfAchievement = new DateOnly(1994, 8, 23) } 
                }))
            .WithId(EntityId<Person>.Parse("00000000-0000-0000-0000-000000000005")),
        FunctionalHelper.FromResult(
            Person.Create(
                new PersonResource{ 
                    Forename = "David",
                    Surname = "Bowie", 
                    DateOfBirth = new DateOnly(1944, 7, 7), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000004"),
                    GreatestAchievement = new PersonalAchievementResource { Description = "Hunky Dory", DateOfAchievement = new DateOnly(1971, 12, 17) } 
                }))
            .WithId(EntityId<Person>.Parse("00000000-0000-0000-0000-000000000006")),
        FunctionalHelper.FromResult(
            Person.Create(
                new PersonResource{ 
                    Forename = "Kobe",
                    Surname = "Bryant", 
                    DateOfBirth = new DateOnly(1984, 7, 7), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000004"),
                    GreatestAchievement = new PersonalAchievementResource { Description = "5 championships with the Lakers", DateOfAchievement = new DateOnly(2010, 6, 17) } 
                }))
            .WithId(EntityId<Person>.Parse("00000000-0000-0000-0000-000000000007")),
        FunctionalHelper.FromResult(
            Person.Create(
                new PersonResource{ 
                    Forename = "John",
                    Surname = "Madden", 
                    DateOfBirth = new DateOnly(1964, 7, 7), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000004"),
                    GreatestAchievement = new PersonalAchievementResource { Description = "SuperBowl XI win with the Raiders", DateOfAchievement = new DateOnly(1977, 1, 9) } 
                }))
            .WithId(EntityId<Person>.Parse("00000000-0000-0000-0000-000000000008")),
        FunctionalHelper.FromResult(
            Person.Create(
                new PersonResource{ 
                    Forename = "Jimmy",
                    Surname = "Greaves", 
                    DateOfBirth = new DateOnly(1940, 2, 20), 
                    HomeAddressId = EntityId<Address>.Parse("00000000-0000-0000-0001-000000000004"),
                    GreatestAchievement = new PersonalAchievementResource { Description = "266 goals for Spurs", DateOfAchievement = new DateOnly(1971, 1, 1) } 
                }))
            .WithId(EntityId<Person>.Parse("00000000-0000-0000-0000-000000000009"))
    };

    public static readonly List<Address> Addresses = new()
    {
        FunctionalHelper.FromResult(Address.Create(new AddressResource { Line1 = "Some Street", Line2 = "Svartalfheim", PostCode = "SV1 1SS" })).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000001")),
        FunctionalHelper.FromResult(Address.Create(new AddressResource { Line1 = "Awful Avenue", Line2 = "Alfheim", PostCode = "AL2 2AA" })).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000002")),
        FunctionalHelper.FromResult(Address.Create(new AddressResource { Line1 = "Manky Mews", Line2 = "Midgard", PostCode = "MI3 3MM" })).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000003")),
        FunctionalHelper.FromResult(Address.Create(new AddressResource { Line1 = "Beyond", Line2 = "The Ether", PostCode = "BE1 0ND" })).WithId(EntityId<Address>.Parse("00000000-0000-0000-0001-000000000004"))
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