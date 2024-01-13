using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class EmploymentTypeValueConverter : ValueConverter<EmploymentType, string>
{
    public EmploymentTypeValueConverter()
        : base(valueObject => valueObject.Value,
            value => EmploymentType.Convert(value))
    {
    }
}