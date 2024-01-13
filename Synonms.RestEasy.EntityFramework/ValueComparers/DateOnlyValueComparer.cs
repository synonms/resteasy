using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Synonms.RestEasy.EntityFramework.ValueComparers;

public class DateOnlyValueComparer : ValueComparer<DateOnly>
{
    public DateOnlyValueComparer() : base(
        (d1, d2) => d1.DayNumber == d2.DayNumber,
        d => d.GetHashCode())
    {
    }
}
