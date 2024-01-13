using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class MilesValueConverter : ValueConverter<Miles, int>
{
    public MilesValueConverter()
        : base(valueObject => valueObject.Value,
            value => Miles.Convert(value))
    {
    }
}