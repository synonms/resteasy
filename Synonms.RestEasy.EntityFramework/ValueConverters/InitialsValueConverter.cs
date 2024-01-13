using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class InitialsValueConverter : ValueConverter<Initials, string>
{
    public InitialsValueConverter()
        : base(valueObject => valueObject.Value,
            value => Initials.Convert(value))
    {
    }
}