using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class DescriptionValueConverter : ValueConverter<Description, string>
{
    public DescriptionValueConverter()
        : base(valueObject => valueObject.Value,
            value => Description.Convert(value))
    {
    }
}