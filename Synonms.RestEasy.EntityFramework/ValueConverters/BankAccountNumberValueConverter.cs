using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class BankAccountNumberValueConverter : ValueConverter<BankAccountNumber, string>
{
    public BankAccountNumberValueConverter()
        : base(valueObject => valueObject.Value,
            value => BankAccountNumber.Convert(value))
    {
    }
}