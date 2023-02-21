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
    
    private Pet(EntityId<Pet> id, EntityId<Person> ownerId, RestEasyMoniker name, RestEasyEventDate dateOfBirth)
        : this(ownerId, name, dateOfBirth)
    {
        Id = id;
    }
    
    private Pet(EntityId<Person> ownerId, RestEasyMoniker name, RestEasyEventDate dateOfBirth)
    {
        OwnerId = ownerId;
        Name = name;
        DateOfBirth = dateOfBirth;
    }
    
    public EntityId<Person> OwnerId { get; private set; }
    
    public RestEasyMoniker Name { get; private set; }
    
    public RestEasyEventDate DateOfBirth { get; private set; }
    
    public static Result<Pet> Create(PetResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Name, x => RestEasyMoniker.CreateMandatory(x, NameMaxLength), out RestEasyMoniker nameValueObject)
            .WithMandatoryValueObject(resource.DateOfBirth, RestEasyEventDate.CreateMandatory, out RestEasyEventDate dateOfBirthValueObject)
            .WithDomainRules(
                RelatedEntityIdRules<Person>.Create(nameof(OwnerId), resource.OwnerId)
                )
            .Build()
            .ToResult(new Pet(resource.OwnerId, nameValueObject, dateOfBirthValueObject));

    public Maybe<Fault> Update(PetResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Name, x => RestEasyMoniker.CreateMandatory(x, NameMaxLength), out RestEasyMoniker nameValueObject)
            .WithMandatoryValueObject(resource.DateOfBirth, RestEasyEventDate.CreateMandatory, out RestEasyEventDate dateOfBirthValueObject)
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