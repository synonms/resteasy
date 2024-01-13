using Synonms.RestEasy.Core.Domain.ValueObjects;
using Synonms.RestEasy.Core.Extensions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class EventTimeValueConverter : ValueConverter<EventTime, DateTime>
{
    public EventTimeValueConverter()
        : base(valueObject => valueObject.Value.ToDateTime(DateOnly.MinValue),
            value => EventTime.Convert(TimeOnly.FromDateTime(value)))
    {
    }
}