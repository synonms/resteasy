using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class SexValueConverter : ValueConverter<Sex, string>
{
    public SexValueConverter()
        : base(valueObject => valueObject.Value,
            value => Sex.Convert(value))
    {
    }
}