using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class DiscriminatorValueConverter : ValueConverter<Discriminator, string>
{
    public DiscriminatorValueConverter()
        : base(valueObject => valueObject.Value,
            value => Discriminator.Convert(value))
    {
    }
}