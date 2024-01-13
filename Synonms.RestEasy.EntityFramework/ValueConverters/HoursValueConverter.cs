using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class HoursValueConverter : ValueConverter<Hours, int>
{
    public HoursValueConverter()
        : base(valueObject => valueObject.Value,
            value => Hours.Convert(value))
    {
    }
}