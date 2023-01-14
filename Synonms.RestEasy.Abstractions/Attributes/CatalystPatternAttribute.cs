namespace Juniper.Catalyst.Rest.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class CatalystPatternAttribute : Attribute
{
    public string Pattern { get; }

    public CatalystPatternAttribute(string pattern)
    {
        Pattern = pattern;
    }
}