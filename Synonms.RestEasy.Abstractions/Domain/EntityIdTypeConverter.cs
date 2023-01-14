﻿using System.ComponentModel;
using System.Globalization;

namespace Synonms.RestEasy.Abstractions.Domain;

public class EntityIdTypeConverter<TEntity> : TypeConverter
    where TEntity : Entity<TEntity>
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string)
            || sourceType == typeof(Guid)
            || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string)
            || destinationType == typeof(Guid)
            || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        Guid? guid = value switch
        {
            string s => Guid.TryParse(s, out Guid g) ? g : null,
            Guid g => g,
            _ => null
        };

        if (guid is not null)
        {
            EntityId<TEntity> entityId = new EntityId<TEntity>(guid.Value);
            return entityId;
        }
        
        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is null)
        {
            return null;
        }

        EntityId<TEntity> entityId = (EntityId<TEntity>)value;
        Guid guid = entityId.Value;

        if (destinationType == typeof(string))
        {
            return guid.ToString();
        }

        if (destinationType == typeof(Guid))
        {
            return guid;
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
