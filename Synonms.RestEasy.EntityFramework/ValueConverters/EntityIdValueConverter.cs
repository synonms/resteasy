using System.Reflection;
using Synonms.RestEasy.Core.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class EntityIdValueConverter<TEntity> : ValueConverter<EntityId<TEntity>, Guid>
    where TEntity : Entity<TEntity>
{
    public EntityIdValueConverter()
        : base(uniqueId => ConvertToFunc(uniqueId), guid => ConvertFromFunc(guid)) 
    {
    }

    private static readonly Func<EntityId<TEntity>, Guid> ConvertToFunc = uniqueId => uniqueId.Value;
    private static readonly Func<Guid, EntityId<TEntity>> ConvertFromFunc = guid =>
    {
        ConstructorInfo? constructorInfo = typeof(EntityId<TEntity>).GetConstructor(BindingFlags.Instance | BindingFlags.Public, new Type[] { typeof(Guid) });

        if (constructorInfo is null)
        {
            throw new InvalidOperationException($"Constructor {nameof(EntityId<TEntity>)}(Guid) is not defined.");
        }

        EntityId<TEntity>? entityId = constructorInfo.Invoke(new object?[] { guid }) as EntityId<TEntity>;

        return entityId ?? throw new NullReferenceException($"Unable to construct {nameof(EntityId<TEntity>)}({guid})");
    };
}