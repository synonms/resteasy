using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;

namespace Synonms.RestEasy.Abstractions.Domain;

public abstract class Entity<TEntity> : IDomainEventProducer
    where TEntity : Entity<TEntity>
{
    [NotMapped]
    private readonly ConcurrentQueue<DomainEvent> _domainEvents = new ();

    public EntityId<TEntity> Id { get; protected init; } = EntityId<TEntity>.New();

    [NotMapped]
    public IProducerConsumerCollection<DomainEvent> DomainEvents => _domainEvents;
    
    protected void ProduceEvent(DomainEvent domainEvent)
    {
        _domainEvents.Enqueue(domainEvent);
    }
    
    public override bool Equals(object? obj)
    {
        if ((obj is Entity<TEntity> other) is false)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        if (Id.IsEmpty || other.Id.IsEmpty)
        {
            return false;
        }

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

    public static bool operator ==(Entity<TEntity>? left, Entity<TEntity>? right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
        {
            return true;
        }

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Entity<TEntity>? left, Entity<TEntity>? right) =>
        !(left == right);
}