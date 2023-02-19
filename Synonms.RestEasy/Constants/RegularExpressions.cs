namespace Synonms.RestEasy.Constants;

public static class RegularExpressions
{
    public const string Guid = @"^[0-9a-fA-F]{8}[-]([0-9a-fA-F]{4}[-]){3}[0-9a-fA-F]{12}$";
    public const string DateOnly = @"^\d{4}-\d{2}-\d{2}$";
    public const string DateTime = @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$";
    public const string PostCode = @"^[A-Z]{1,2}\d[A-Z\d]? ?\d[A-Z]{2}$";
    public const string Email = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$";
}