using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class UnitsValueConverter : ValueConverter<Units, int>
{
    public UnitsValueConverter()
        : base(valueObject => valueObject.Value,
            value => Units.Convert(value))
    {
    }
}