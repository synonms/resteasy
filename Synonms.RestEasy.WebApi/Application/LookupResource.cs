namespace Synonms.RestEasy.WebApi.Application;

public class LookupResource
{
    public Guid Id { get; set; } = Guid.Empty;
    
    public string LookupCode { get; set; } = string.Empty;
    
    public string LookupName { get; set; } = string.Empty;
}