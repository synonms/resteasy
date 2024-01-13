using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class DaysValueConverter : ValueConverter<Days, int>
{
    public DaysValueConverter()
        : base(valueObject => valueObject.Value,
            value => Days.Convert(value))
    {
    }
}