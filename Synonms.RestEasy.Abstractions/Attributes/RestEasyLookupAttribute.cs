namespace Synonms.RestEasy.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RestEasyLookupAttribute : Attribute
{
    public RestEasyLookupAttribute(string discriminator)
    {
        Discriminator = discriminator;
    }
    
    public string Discriminator { get; }
}
