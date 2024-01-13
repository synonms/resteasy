using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class TelephoneNumberValueConverter : ValueConverter<TelephoneNumber, string>
{
    public TelephoneNumberValueConverter()
        : base(valueObject => valueObject.Value,
            value => TelephoneNumber.Convert(value))
    {
    }
}