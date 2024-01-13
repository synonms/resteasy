using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class IsAValueConverter : ValueConverter<IsA, bool>
{
    public IsAValueConverter()
        : base(valueObject => valueObject.Value,
            value => IsA.Convert(value))
    {
    }
}