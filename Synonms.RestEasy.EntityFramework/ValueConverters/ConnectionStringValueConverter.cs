using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Synonms.RestEasy.Core.Domain.ValueObjects;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class ConnectionStringValueConverter : ValueConverter<ConnectionString, string>
{
    public ConnectionStringValueConverter()
        : base(valueObject => valueObject.Value,
            value => ConnectionString.Convert(value))
    {
    }
}