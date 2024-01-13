using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class AddressLineValueConverter : ValueConverter<AddressLine, string>
{
    public AddressLineValueConverter()
        : base(valueObject => valueObject.Value,
            value => AddressLine.Convert(value))
    {
    }
}