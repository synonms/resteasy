using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class GeneralLedgerCodeValueConverter : ValueConverter<GeneralLedgerCode, string>
{
    public GeneralLedgerCodeValueConverter()
        : base(valueObject => valueObject.Value,
            value => GeneralLedgerCode.Convert(value))
    {
    }
}