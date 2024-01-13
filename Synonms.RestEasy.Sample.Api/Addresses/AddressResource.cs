using Synonms.RestEasy.WebApi.Schema;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.Sample.Api.Addresses;

public class AddressResource : Resource
{
    public AddressResource()
    {
    }
    
    public AddressResource(Guid id, Link selfLink) 
        : base(id, selfLink)
    {
    }
    
    public string Line1 { get; set; } = string.Empty;
    
    public string Line2 { get; set; } = string.Empty;
    
    public string PostCode { get; set; } = string.Empty;
}