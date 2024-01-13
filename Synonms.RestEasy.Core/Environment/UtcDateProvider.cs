namespace Synonms.RestEasy.Core.Environment;

public class UtcDateProvider : IDateProvider
{
    public DateTime Now() => 
        DateTime.UtcNow;
    
    public DateOnly Today() =>
        DateOnly.FromDateTime(DateTime.Today);
}