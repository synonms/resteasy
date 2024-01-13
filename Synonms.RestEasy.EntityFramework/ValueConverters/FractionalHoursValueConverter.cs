using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class FractionalHoursValueConverter : ValueConverter<FractionalHours, decimal>
{
    public FractionalHoursValueConverter()
        : base(valueObject => valueObject.Value,
            value => FractionalHours.Convert(value))
    {
    }
}