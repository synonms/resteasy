using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class WeekNumberValueConverter : ValueConverter<WeekNumber, int>
{
    public WeekNumberValueConverter()
        : base(valueObject => valueObject.Value,
            value => WeekNumber.Convert(value))
    {
    }
}