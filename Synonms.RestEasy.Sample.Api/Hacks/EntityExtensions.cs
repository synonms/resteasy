using System.Reflection;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Sample.Api.Hacks;

public static class EntityExtensions
{
    public static TEntity WithId<TEntity>(this TEntity entity, EntityId<TEntity> entityId)
        where TEntity : Entity<TEntity> 
    {
        PropertyInfo? propertyInfo = typeof(TEntity).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public);

        if (propertyInfo is null)
        {
            throw new NullReferenceException($"Unable to get Id property of {nameof(TEntity)}.");
        }
        
        propertyInfo.SetValue(entity, entityId);

        return entity;
    }
}