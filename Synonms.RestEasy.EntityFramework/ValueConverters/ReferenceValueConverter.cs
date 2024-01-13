using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class ReferenceValueConverter : ValueConverter<Reference, string>
{
    public ReferenceValueConverter()
        : base(valueObject => valueObject.Value,
            value => Reference.Convert(value))
    {
    }
}