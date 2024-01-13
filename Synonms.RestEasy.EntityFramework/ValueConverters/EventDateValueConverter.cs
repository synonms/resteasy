using Synonms.RestEasy.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class EventDateValueConverter : ValueConverter<EventDate, DateTime>
{
    public EventDateValueConverter()
        : base(valueObject => valueObject.Value.ToDateTime(TimeOnly.MinValue),
            value => EventDate.Convert(DateOnly.FromDateTime(value)))
    {
    }
}