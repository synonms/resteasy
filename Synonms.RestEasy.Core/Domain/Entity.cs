using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using Synonms.RestEasy.Core.Domain.Events;
using Synonms.RestEasy.Core.Domain.ValueObjects;

namespace Synonms.RestEasy.Core.Domain;

public abstract class Entity<TEntity> : IDomainEventProducer
    where TEntity : Entity<TEntity>
{
    [NotMapped]
    private readonly ConcurrentQueue<DomainEvent> _domainEvents = new ();

    public EntityId<TEntity> Id { get; protected init; } = EntityId<TEntity>.New();

    public IsA IsActive { get; protected set; } = IsA.Yes;
    
    public DateTime CreatedAt { get; protected init; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; protected set; }

    [NotMapped]
    public IProducerConsumerCollection<DomainEvent> DomainEvents => _domainEvents;
    
    protected virtual void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

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
    
    protected void UpdateMandatoryValue<TValueObject>(Expression<Func<TEntity, TValueObject>> property, TValueObject newValue) 
        where TValueObject : ValueObject
    {
        if (TryGetMandatoryValue(property, out TValueObject? originalValue) is false)
        {
            return;
        }

        if (newValue == originalValue)
        {
            return;
        }

        TrySetValue(property, newValue);
    }
    
    protected void UpdateMandatoryValue<TOtherEntity>(Expression<Func<TEntity, EntityId<TOtherEntity>>> property, EntityId<TOtherEntity> newValue) 
        where TOtherEntity : Entity<TOtherEntity>
    {
        if (TryGetMandatoryValue(property, out EntityId<TOtherEntity>? originalValue) is false)
        {
            return;
        }

        if (newValue == originalValue)
        {
            return;
        }

        TrySetValue(property, newValue);
    }

    protected void UpdateOptionalValue<TValueObject>(Expression<Func<TEntity, TValueObject?>> property, TValueObject? newValue) 
        where TValueObject : ValueObject
    {
        if (TryGetOptionalValue(property, out TValueObject? originalValue) is false)
        {
            return;
        }

        if (newValue == originalValue)
        {
            return;
        }

        TrySetValue(property, newValue);
    }
    
    protected void UpdateOptionalValue<TOtherEntity>(Expression<Func<TEntity, EntityId<TOtherEntity>?>> property, EntityId<TOtherEntity>? newValue) 
        where TOtherEntity : Entity<TOtherEntity>
    {
        if (TryGetOptionalValue(property, out EntityId<TOtherEntity>? originalValue) is false)
        {
            return;
        }

        if (newValue == originalValue)
        {
            return;
        }

        TrySetValue(property, newValue);
    }

    private static bool TryGetPropertyInfo<T>(Expression<Func<TEntity, T>> property, out PropertyInfo? propertyInfo)
    {
        propertyInfo = null;
        
        if (property.Body is not MemberExpression memberExpression)
        {
            return false;
        }

        if (memberExpression.Member is not PropertyInfo memberExpressionAsPropertyInfo)
        {
            return false;
        }

        propertyInfo = memberExpressionAsPropertyInfo;
        return true;
    }

    private bool TryGetMandatoryValue<T>(Expression<Func<TEntity, T>> property, out T? value)
    {
        value = default;

        if (TryGetPropertyInfo(property, out PropertyInfo? propertyInfo) is false || propertyInfo is null)
        {
            return false;
        }
        
        object? extractedValue = propertyInfo.GetValue(this);

        if (extractedValue is not T extractedValueAsT)
        {
            return false;
        }

        value = extractedValueAsT;
        return true;
    }

    private bool TryGetOptionalValue<T>(Expression<Func<TEntity, T?>> property, out T? value)
    {
        value = default;

        if (TryGetPropertyInfo(property, out PropertyInfo? propertyInfo) is false || propertyInfo is null)
        {
            return false;
        }
        
        object? extractedValue = propertyInfo.GetValue(this);

        if (extractedValue is null)
        {
            return true;
        }
        
        if (extractedValue is not T extractedValueAsT)
        {
            return false;
        }

        value = extractedValueAsT;
        return true;
    }

    private bool TrySetValue<T>(Expression<Func<TEntity, T>> property, T? value)
    {
        if (TryGetPropertyInfo(property, out PropertyInfo? propertyInfo) is false || propertyInfo is null)
        {
            return false;
        }

        if (propertyInfo.CanWrite is false)
        {
            return false;
        }

        propertyInfo.SetValue(this, value);
        
        MarkAsUpdated();

        return true;
    }
}