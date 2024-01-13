using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class MonthNumberValueConverter : ValueConverter<MonthNumber, int>
{
    public MonthNumberValueConverter()
        : base(valueObject => valueObject.Value,
            value => MonthNumber.Convert(value))
    {
    }
}