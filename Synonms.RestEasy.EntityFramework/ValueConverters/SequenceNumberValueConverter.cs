using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class SequenceNumberValueConverter : ValueConverter<SequenceNumber, int>
{
    public SequenceNumberValueConverter()
        : base(valueObject => valueObject.Value,
            value => SequenceNumber.Convert(value))
    {
    }
}