using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class PaymentFrequencyValueConverter : ValueConverter<PaymentFrequency, string>
{
    public PaymentFrequencyValueConverter()
        : base(valueObject => valueObject.Value,
            value => PaymentFrequency.Convert(value))
    {
    }
}