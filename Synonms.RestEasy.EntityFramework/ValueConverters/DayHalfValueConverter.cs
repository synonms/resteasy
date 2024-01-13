using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class DayHalfValueConverter : ValueConverter<DayHalf, string>
{
    public DayHalfValueConverter()
        : base(valueObject => valueObject.Value,
            value => DayHalf.Convert(value))
    {
    }
}