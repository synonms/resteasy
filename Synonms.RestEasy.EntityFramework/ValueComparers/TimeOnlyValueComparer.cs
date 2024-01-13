using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Synonms.RestEasy.EntityFramework.ValueComparers;

public class TimeOnlyValueComparer : ValueComparer<TimeOnly>
{
    public TimeOnlyValueComparer() : base(
        (t1, t2) => t1.Ticks == t2.Ticks,
        t => t.GetHashCode())
    {
    }
}