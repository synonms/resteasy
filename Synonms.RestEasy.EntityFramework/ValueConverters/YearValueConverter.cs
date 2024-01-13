using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class YearValueConverter : ValueConverter<Year, int>
{
    public YearValueConverter()
        : base(valueObject => valueObject.Value,
            value => Year.Convert(value))
    {
    }
}