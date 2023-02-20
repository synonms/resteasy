using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Domain;
using Synonms.RestEasy.Domain.ValueObjects;

namespace Synonms.RestEasy.Sample.Api.People;

public class PersonalAchievement : AggregateMember<PersonalAchievement>
{
    public const int DescriptionMaxLength = 250;

    private PersonalAchievement(EntityId<PersonalAchievement> id, Description description, EventDate dateOfAchievement)
        : this(description, dateOfAchievement)
    {
        Id = id;
    }

    private PersonalAchievement(Description description, EventDate dateOfAchievement)
    {
        Description = description;
        DateOfAchievement = dateOfAchievement;
    }

    public Description Description { get; private set; }

    public EventDate DateOfAchievement { get; private set; }

    public static Result<PersonalAchievement> Create(PersonalAchievementResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Description, x => Description.CreateMandatory(x, DescriptionMaxLength), out Description descriptionValueObject)
            .WithMandatoryValueObject(resource.DateOfAchievement, EventDate.CreateMandatory, out EventDate dateOfAchievementValueObject)
            .Build()
            .ToResult(new PersonalAchievement(descriptionValueObject, dateOfAchievementValueObject));

    public Maybe<Fault> Update(PersonalAchievementResource resource) =>
        AggregateRules.CreateBuilder()
            .WithMandatoryValueObject(resource.Description, x => Description.CreateMandatory(x, DescriptionMaxLength), out Description descriptionValueObject)
            .WithMandatoryValueObject(resource.DateOfAchievement, EventDate.CreateMandatory, out EventDate dateOfAchievementValueObject)
            .Build()
            .BiBind(() =>
            {
                Description = descriptionValueObject;
                DateOfAchievement = dateOfAchievementValueObject;

                return Maybe<Fault>.None;
            });
}