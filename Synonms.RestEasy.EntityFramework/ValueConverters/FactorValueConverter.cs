using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class FactorValueConverter : ValueConverter<Factor, decimal>
{
    public FactorValueConverter()
        : base(valueObject => valueObject.Value,
            value => Factor.Convert(value))
    {
    }
}