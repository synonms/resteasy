using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Server;

namespace Synonms.RestEasy.Sample.Api.Addresses;

public class AddressResource : ServerResource<Address>
{
    public AddressResource()
    {
    }
    
    public AddressResource(EntityId<Address> id, Link selfLink) 
        : base(id, selfLink)
    {
    }
    
    public string Line1 { get; set; } = string.Empty;
    
    public string Line2 { get; set; } = string.Empty;
    
    public string PostCode { get; set; } = string.Empty;
}