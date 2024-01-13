using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class PercentValueConverter : ValueConverter<Percent, decimal>
{
    public PercentValueConverter()
        : base(valueObject => valueObject.Value,
            value => Percent.Convert(value))
    {
    }
}