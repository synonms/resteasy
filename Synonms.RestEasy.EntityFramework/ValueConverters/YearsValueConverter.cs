using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class YearsValueConverter : ValueConverter<Years, int>
{
    public YearsValueConverter()
        : base(valueObject => valueObject.Value,
            value => Years.Convert(value))
    {
    }
}