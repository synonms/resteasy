namespace Synonms.RestEasy.Abstractions.Domain;

public abstract class AggregateRoot<TAggregateRoot> : Entity<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    public EntityTag EntityTag { get; protected init; } = EntityTag.Uninitialised;
    
    public DateTime CreatedAt { get; private set; } = DateTime.Now; 
}
