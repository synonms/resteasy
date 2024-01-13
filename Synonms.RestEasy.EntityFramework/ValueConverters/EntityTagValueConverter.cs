using Synonms.RestEasy.Core.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class EntityTagValueConverter : ValueConverter<EntityTag, Guid>
{
    public EntityTagValueConverter()
        : base(
            x => x.Value,
            x => x)
    {
    }
}