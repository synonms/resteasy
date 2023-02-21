using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.Abstractions.Attributes;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain;
using Synonms.RestEasy.Domain.Rules;
using Synonms.RestEasy.Domain.ValueObjects;
using Synonms.RestEasy.Extensions;
using Synonms.RestEasy.Sample.Api.Addresses;

namespace Synonms.RestEasy.Sample.Api.People;

[RestEasyResource("people")]
public class Person : AggregateRoot<Person>
{
    public const int ForenameMaxLength = 30;
    public const int SurnameMaxLength = 30;
    public const int ColourMaxLength = 10;
    
    private Person(EntityId<Person> id, RestEasyMoniker forename, RestEasyMoniker surname, RestEasyEventDate dateOfBirth, RestEasyColour? favouriteColour, EntityId<Address> homeAddressId, PersonalAchievement greatestAchievement, ICollection<PersonalAchievement> achievements)
        : this(forename, surname, dateOfBirth, favouriteColour, homeAddressId, greatestAchievement, achievements)
    {
        Id = id;
    }
    
    private Person(RestEasyMoniker forename, RestEasyMoniker surname, RestEasyEventDate dateOfBirth, RestEasyColour? favouriteColour, EntityId<Address> homeAddressId, PersonalAchievement greatestAchievement, ICollection<PersonalAchievement> achievements)
    {
        Forename = forename;
        Surname = surname;
        DateOfBirth = dateOfBirth;
        FavouriteColour = favouriteColour;
        HomeAddressId = homeAddressId;
        GreatestAchievement = greatestAchievement;
        Achievements = achievements;
    }
    
    public RestEasyMoniker Forename { get; private set; }
    
    public RestEasyMoniker Surname { get; private set; }
    
    public RestEasyEventDate DateOfBirth { get; private set; }
    
    public RestEasyColour? FavouriteColour { get; private set; }

    // Related resource (presents as a link)
    public EntityId<Address> HomeAddressId { get; private set; }

    // Nested resource (presents as a populated child resource object)
    public PersonalAchievement GreatestAchievement { get; private set; }

    // Nested resource collection (presents as a populated array of child resources)
    public ICollection<PersonalAchievement> Achievements { get; private set; }

    public static Result<Person> Create(PersonResource resource) =>
        resource.Achievements
            .Select(PersonalAchievement.Create)
            .Reduce(achievements => achievements)
            .Bind(
                achievements =>
                    PersonalAchievement.Create(resource.GreatestAchievement)
                        .Bind(greatestAchievement =>
                            AggregateRules.CreateBuilder()
                                .WithMandatoryValueObject(resource.Forename, x => RestEasyMoniker.CreateMandatory(x, ForenameMaxLength), out RestEasyMoniker forenameValueObject)
                                .WithMandatoryValueObject(resource.Surname, x => RestEasyMoniker.CreateMandatory(x, SurnameMaxLength), out RestEasyMoniker surnameValueObject)
                                .WithMandatoryValueObject(resource.DateOfBirth, RestEasyEventDate.CreateMandatory, out RestEasyEventDate dateOfBirthValueObject)
                                .WithOptionalValueObject(resource.FavouriteColour, x => RestEasyColour.CreateOptional(x, ColourMaxLength), out RestEasyColour? favouriteColourValueObject)
                                .WithDomainRules(
                                    RelatedEntityIdRules<Address>.Create(nameof(HomeAddressId), resource.HomeAddressId)
                                    )
                                .Build()
                                .ToResult(new Person(forenameValueObject, surnameValueObject, dateOfBirthValueObject, favouriteColourValueObject, resource.HomeAddressId, greatestAchievement, achievements.ToList()))));

    public Maybe<Fault> Update(PersonResource resource) =>
        MergeAchievements(resource)
            .BiBind(() => 
                PersonalAchievement.Create(resource.GreatestAchievement)
                    .Bind(greatestAchievement =>
                        AggregateRules.CreateBuilder()
                            .WithMandatoryValueObject(resource.Forename, x => RestEasyMoniker.CreateMandatory(x, ForenameMaxLength), out RestEasyMoniker forenameValueObject)
                            .WithMandatoryValueObject(resource.Surname, x => RestEasyMoniker.CreateMandatory(x, SurnameMaxLength), out RestEasyMoniker surnameValueObject)
                            .WithMandatoryValueObject(resource.DateOfBirth, RestEasyEventDate.CreateMandatory, out RestEasyEventDate dateOfBirthValueObject)
                            .WithOptionalValueObject(resource.FavouriteColour, x => RestEasyColour.CreateOptional(x, ColourMaxLength), out RestEasyColour? favouriteColourValueObject)
                            .WithDomainRules(
                                RelatedEntityIdRules<Address>.Create(nameof(HomeAddressId), resource.HomeAddressId)
                            )
                            .Build()
                            .BiBind(() =>
                            {
                                Forename = forenameValueObject;
                                Surname = surnameValueObject;
                                DateOfBirth = dateOfBirthValueObject;
                                FavouriteColour = favouriteColourValueObject;
                                HomeAddressId = resource.HomeAddressId;
                                GreatestAchievement = greatestAchievement;

                                return Maybe<Fault>.None;
                            })));
    
    private Maybe<Fault> MergeAchievements(PersonResource resource) =>
        Achievements
            .Merge<PersonalAchievement, PersonalAchievementResource>(
                resource.Achievements,
                (am, r) => am.Id == r.Id,
                PersonalAchievement.Create,
                (am, r) => am.Update(r));
}