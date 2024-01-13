using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class FileNameValueConverter: ValueConverter<FileName, string>
{
    public FileNameValueConverter()
        : base(valueObject => valueObject.Value,
            value => FileName.Convert(value))
    {
    }
}