namespace Synonms.RestEasy.Abstractions.Domain;

public abstract class DomainEvent : Entity<DomainEvent>
{
    public DateTime CreatedAt { get; protected init; } = DateTime.UtcNow;
}