using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class HasAValueConverter : ValueConverter<HasA, bool>
{
    public HasAValueConverter()
        : base(valueObject => valueObject.Value,
            value => HasA.Convert(value))
    {
    }
}