using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class TitleValueConverter : ValueConverter<Title, string>
{
    public TitleValueConverter()
        : base(valueObject => valueObject.Value,
            value => Title.Convert(value))
    {
    }
}