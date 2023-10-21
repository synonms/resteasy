namespace Synonms.RestEasy.Abstractions.Domain;

public abstract class Lookup : Entity<Lookup>
{
    public bool IsActive { get; protected set; } = true;

    public string LookupCode { get; init; } = string.Empty;
    
    public string LookupName { get; init; } = string.Empty;
}
