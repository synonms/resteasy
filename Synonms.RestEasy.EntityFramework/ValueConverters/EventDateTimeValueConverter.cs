using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class EventDateTimeValueConverter : ValueConverter<EventDateTime, DateTime>
{
    public EventDateTimeValueConverter()
        : base(valueObject => valueObject.Value,
            value => EventDateTime.Convert(value))
    {
    }
}