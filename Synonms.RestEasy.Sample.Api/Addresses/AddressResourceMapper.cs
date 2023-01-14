using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Sample.Api.Addresses;

public class AddressResourceMapper : IResourceMapper<Address, AddressResource>
{
    private readonly IRouteGenerator _routeGenerator;

    public AddressResourceMapper(IRouteGenerator routeGenerator)
    {
        _routeGenerator = routeGenerator;
    }
    
    public AddressResource Map(HttpContext httpContext, Address aggregateRoot)
    {
        Uri selfUri = _routeGenerator.Item(httpContext, aggregateRoot.Id);
        Link selfLink = Link.SelfLink(selfUri);
        
        return new AddressResource(
            aggregateRoot.Id, 
            selfLink,
            aggregateRoot.Line1,
            aggregateRoot.Line2,
            aggregateRoot.PostCode
        );
    }
}