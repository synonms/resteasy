namespace Synonms.RestEasy.Core.Domain.Rules.Rulesets;

public static class ErrorTemplates
{
    public const string MinValueError = "{0} must be {1} or more.";
    public const string MaxValueError = "{0} must be {1} or less.";
    public const string EndDateBeforeStartDateError = "{0} must be on or after corresponding start date '{1}'.";
    public const string PatternError = "{0} must match regular expression '{1}'.";
}