using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.Abstractions.Attributes;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain;
using Synonms.RestEasy.Domain.Rules;
using Synonms.RestEasy.Domain.ValueObjects;
using Synonms.RestEasy.Sample.Api.Addresses;

namespace Synonms.RestEasy.Sample.Api.People;

[RestEasyResource("people")]
public class Person : AggregateRoot<Person>
{
    public const int ForenameMaxLength = 30;
    public const int SurnameMaxLength = 30;
    public const int ColourMaxLength = 10;
    
    private Person(EntityId<Person> id, Moniker forename, Moniker surname, EventDate dateOfBirth, Colour? favouriteColour, EntityId<Address> homeAddressId)
        : this(forename, surname, dateOfBirth, favouriteColour, homeAddressId)
    {
        Id = id;
    }
    
    private Person(Moniker forename, Moniker surname, EventDate dateOfBirth, Colour? favouriteColour, EntityId<Address> homeAddressId)
    {
        Forename = forename;
        Surname = surname;
        DateOfBirth = dateOfBirth;
        FavouriteColour = favouriteColour;
        HomeAddressId = homeAddressId;
    }
    
    public Moniker Forename { get; private set; }
    
    public Moniker Surname { get; private set; }
    
    public EventDate DateOfBirth { get; private set; }
    
    public Colour? FavouriteColour { get; private set; }
    
    public EntityId<Address> HomeAddressId { get; private set; }

    // TODO: Present Pets as a link (without a navigation property?)
    
    public static Result<Person> Create(PersonResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Forename, x => Moniker.CreateMandatory(x, ForenameMaxLength), out Moniker forenameValueObject)
            .WithMandatoryValueObject(resource.Surname, x => Moniker.CreateMandatory(x, SurnameMaxLength), out Moniker surnameValueObject)
            .WithMandatoryValueObject(resource.DateOfBirth, EventDate.CreateMandatory, out EventDate dateOfBirthValueObject)
            .WithOptionalValueObject(resource.FavouriteColour, x => Colour.CreateOptional(x, ColourMaxLength), out Colour? favouriteColourValueObject)
            .WithDomainRules(
                RelatedEntityIdRules<Address>.Create(nameof(HomeAddressId), resource.HomeAddressId)
                )
            .Build()
            .ToResult(new Person(forenameValueObject, surnameValueObject, dateOfBirthValueObject, favouriteColourValueObject, resource.HomeAddressId));

    public Maybe<Fault> Update(PersonResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Forename, x => Moniker.CreateMandatory(x, ForenameMaxLength), out Moniker forenameValueObject)
            .WithMandatoryValueObject(resource.Surname, x => Moniker.CreateMandatory(x, SurnameMaxLength), out Moniker surnameValueObject)
            .WithMandatoryValueObject(resource.DateOfBirth, EventDate.CreateMandatory, out EventDate dateOfBirthValueObject)
            .WithOptionalValueObject(resource.FavouriteColour, x => Colour.CreateOptional(x, ColourMaxLength), out Colour? favouriteColourValueObject)
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

                return Maybe<Fault>.None;
            });
}