using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class EmailAddressValueConverter : ValueConverter<EmailAddress, string>
{
    public EmailAddressValueConverter()
        : base(valueObject => valueObject.Value,
            value => EmailAddress.Convert(value))
    {
    }
}