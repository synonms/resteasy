using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.Abstractions.Attributes;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain;
using Synonms.RestEasy.Domain.Rules;
using Synonms.RestEasy.Domain.ValueObjects;
using Synonms.RestEasy.Sample.Api.People;

namespace Synonms.RestEasy.Sample.Api.Pets;

[RestEasyResource("pets")]
public class Pet : AggregateRoot<Pet>
{
    private const int NameMaxLength = 30;
    
    private Pet(EntityId<Pet> id, EntityId<Person> ownerId, Moniker name, EventDate dateOfBirth)
        : this(ownerId, name, dateOfBirth)
    {
        Id = id;
    }
    
    private Pet(EntityId<Person> ownerId, Moniker name, EventDate dateOfBirth)
    {
        OwnerId = ownerId;
        Name = name;
        DateOfBirth = dateOfBirth;
    }
    
    public EntityId<Person> OwnerId { get; private set; }
    
    public Moniker Name { get; private set; }
    
    public EventDate DateOfBirth { get; private set; }
    
    public static Result<Pet> Create(PetResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Name, x => Moniker.CreateMandatory(x, NameMaxLength), out Moniker nameValueObject)
            .WithMandatoryValueObject(resource.DateOfBirth, EventDate.CreateMandatory, out EventDate dateOfBirthValueObject)
            .WithDomainRules(
                RelatedEntityIdRules<Person>.Create(nameof(OwnerId), resource.OwnerId)
                )
            .Build()
            .ToResult(new Pet(resource.OwnerId, nameValueObject, dateOfBirthValueObject));

    public Maybe<Fault> Update(PetResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Name, x => Moniker.CreateMandatory(x, NameMaxLength), out Moniker nameValueObject)
            .WithMandatoryValueObject(resource.DateOfBirth, EventDate.CreateMandatory, out EventDate dateOfBirthValueObject)
            .WithDomainRules(
                RelatedEntityIdRules<Person>.Create(nameof(OwnerId), resource.OwnerId)
            )
            .Build()
            .BiBind(() =>
            {
                OwnerId = resource.OwnerId;
                Name = nameValueObject;
                DateOfBirth = dateOfBirthValueObject;

                return Maybe<Fault>.None;
            });
}