using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class SexValueConverter : ValueConverter<SexAssignedAtBirth, string>
{
    public SexValueConverter()
        : base(valueObject => valueObject.Value,
            value => SexAssignedAtBirth.Convert(value))
    {
    }
}