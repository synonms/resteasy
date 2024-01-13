using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class BankSortCodeValueConverter : ValueConverter<BankSortCode, string>
{
    public BankSortCodeValueConverter()
        : base(valueObject => valueObject.Value,
            value => BankSortCode.Convert(value))
    {
    }
}