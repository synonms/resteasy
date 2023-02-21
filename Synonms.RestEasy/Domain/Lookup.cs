using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Domain;

public abstract class Lookup : Entity<Lookup>
{
    public string LookupCode { get; init; } = string.Empty;
    
    public string LookupName { get; init; } = string.Empty;
}
