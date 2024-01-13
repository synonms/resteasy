using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class MonthsValueConverter : ValueConverter<Months, int>
{
    public MonthsValueConverter()
        : base(valueObject => valueObject.Value,
            value => Months.Convert(value))
    {
    }
}