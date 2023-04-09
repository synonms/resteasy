using Synonms.RestEasy.Abstractions.Attributes;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Server;
using Synonms.RestEasy.Constants;

namespace Synonms.RestEasy.Sample.Api.People;

public class PersonalAchievementResource : ServerChildResource<PersonalAchievement>
{
    [RestEasyRequired]
    [RestEasyMaxLength(PersonalAchievement.DescriptionMaxLength)]
    public string Description { get; set; } = string.Empty;

    [RestEasyRequired]
    [RestEasyPattern(RegularExpressions.DateOnly)]
    [RestEasyDescriptor(placeholder: Placeholders.DateOnly)]
    public DateOnly DateOfAchievement { get; set; } = DateOnly.MinValue;
}