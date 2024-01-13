using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class PostCodeValueConverter : ValueConverter<PostCode, string>
{
    public PostCodeValueConverter()
        : base(valueObject => valueObject.Value,
            value => PostCode.Convert(value))
    {
    }
}