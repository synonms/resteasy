using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class FractionalDaysValueConverter : ValueConverter<FractionalDays, decimal>
{
    public FractionalDaysValueConverter()
        : base(valueObject => valueObject.Value,
            value => FractionalDays.Convert(value))
    {
    }
}