using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class WeekDayValueConverter : ValueConverter<WeekDay, int>
{
    public WeekDayValueConverter()
        : base(valueObject => valueObject.Value,
            value => WeekDay.Convert(value))
    {
    }
}