using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class CodeValueConverter : ValueConverter<Code, string>
{
    public CodeValueConverter()
        : base(valueObject => valueObject.Value,
            value => Code.Convert(value))
    {
    }
}