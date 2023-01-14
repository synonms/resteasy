using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Sample.Api.Addresses;

public class AddressResource : Resource<Address>
{
    public AddressResource(string line1, string line2, string postCode)
        : this(EntityId<Address>.Uninitialised, Link.EmptyLink(), line1, line2, postCode)
    {
    }
    
    public AddressResource(EntityId<Address> id, Link selfLink, string line1, string line2, string postCode) 
        : base(id, selfLink)
    {
        Line1 = line1;
        Line2 = line2;
        PostCode = postCode;
    }
    
    public string Line1 { get; private set; }
    
    public string Line2 { get; private set; }
    
    public string PostCode { get; private set; }
}