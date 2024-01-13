using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class MonikerValueConverter : ValueConverter<Moniker, string>
{
    public MonikerValueConverter()
        : base(valueObject => valueObject.Value,
            value => Moniker.Convert(value))
    {
    }
}