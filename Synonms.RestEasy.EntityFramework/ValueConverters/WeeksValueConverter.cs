using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class WeeksValueConverter : ValueConverter<Weeks, int>
{
    public WeeksValueConverter()
        : base(valueObject => valueObject.Value,
            value => Weeks.Convert(value))
    {
    }
}