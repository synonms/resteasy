namespace Synonms.RestEasy.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RestEasyPatternAttribute : Attribute
{
    public string Pattern { get; }

    public RestEasyPatternAttribute(string pattern)
    {
        Pattern = pattern;
    }
}