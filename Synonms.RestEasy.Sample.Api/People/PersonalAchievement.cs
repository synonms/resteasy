using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.Abstractions.Attributes;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain;
using Synonms.RestEasy.Domain.ValueObjects;

namespace Synonms.RestEasy.Sample.Api.People;

[RestEasyChildResource(typeof(PersonalAchievementResource))]
public class PersonalAchievement : AggregateMember<PersonalAchievement>
{
    public const int DescriptionMaxLength = 250;

    private PersonalAchievement(EntityId<PersonalAchievement> id, RestEasyDescription description, RestEasyEventDate dateOfAchievement)
        : this(description, dateOfAchievement)
    {
        Id = id;
    }

    private PersonalAchievement(RestEasyDescription description, RestEasyEventDate dateOfAchievement)
    {
        Description = description;
        DateOfAchievement = dateOfAchievement;
    }

    public RestEasyDescription Description { get; private set; }

    public RestEasyEventDate DateOfAchievement { get; private set; }

    public static Result<PersonalAchievement> Create(PersonalAchievementResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Description, x => RestEasyDescription.CreateMandatory(x, DescriptionMaxLength), out RestEasyDescription descriptionValueObject)
            .WithMandatoryValueObject(resource.DateOfAchievement, RestEasyEventDate.CreateMandatory, out RestEasyEventDate dateOfAchievementValueObject)
            .Build()
            .ToResult(new PersonalAchievement(descriptionValueObject, dateOfAchievementValueObject));

    public Maybe<Fault> Update(PersonalAchievementResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Description, x => RestEasyDescription.CreateMandatory(x, DescriptionMaxLength), out RestEasyDescription descriptionValueObject)
            .WithMandatoryValueObject(resource.DateOfAchievement, RestEasyEventDate.CreateMandatory, out RestEasyEventDate dateOfAchievementValueObject)
            .Build()
            .BiBind(() =>
            {
                Description = descriptionValueObject;
                DateOfAchievement = dateOfAchievementValueObject;

                return Maybe<Fault>.None;
            });
}