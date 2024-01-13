using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class FractionalMilesValueConverter : ValueConverter<FractionalMiles, decimal>
{
    public FractionalMilesValueConverter()
        : base(valueObject => valueObject.Value,
            value => FractionalMiles.Convert(value))
    {
    }
}