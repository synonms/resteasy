namespace Synonms.RestEasy.SharedKernel.Extensions;

public static class DateOnlyExtensions
{
    public static DateOnly Today =>
        DateOnly.FromDateTime(DateTime.Today);
}