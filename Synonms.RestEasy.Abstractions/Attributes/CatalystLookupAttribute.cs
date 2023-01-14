namespace Juniper.Catalyst.Rest.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class CatalystLookupAttribute : Attribute
{
    public CatalystLookupAttribute(string discriminator)
    {
        Discriminator = discriminator;
    }
    
    public string Discriminator { get; }
}
