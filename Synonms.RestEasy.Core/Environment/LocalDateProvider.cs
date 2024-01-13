namespace Synonms.RestEasy.Core.Environment;

public class LocalDateProvider : IDateProvider
{
    public DateTime Now() => 
        DateTime.Now;
    
    public DateOnly Today() =>
        DateOnly.FromDateTime(DateTime.Today);
}