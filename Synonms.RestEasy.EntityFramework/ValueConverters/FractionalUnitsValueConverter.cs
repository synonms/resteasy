using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class FractionalUnitsValueConverter : ValueConverter<FractionalUnits, decimal>
{
    public FractionalUnitsValueConverter()
        : base(valueObject => valueObject.Value,
            value => FractionalUnits.Convert(value))
    {
    }
}