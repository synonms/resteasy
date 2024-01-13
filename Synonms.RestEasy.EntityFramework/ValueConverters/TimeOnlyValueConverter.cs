using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Synonms.RestEasy.EntityFramework.ValueConverters;

public class TimeOnlyValueConverter : ValueConverter<TimeOnly, TimeSpan>
{
    public TimeOnlyValueConverter()
        : base(timeOnly => timeOnly.ToTimeSpan(),
            timeSpan => TimeOnly.FromTimeSpan(timeSpan))
    {}
}