namespace Synonms.RestEasy.Core.Domain;

public abstract class Lookup : Entity<Lookup>
{
    public string Discriminator { get; init; } = string.Empty;
    
    public string LookupCode { get; init; } = string.Empty;
    
    public string LookupName { get; init; } = string.Empty;
    
    public bool IsActive { get; protected set; } = true;
}
