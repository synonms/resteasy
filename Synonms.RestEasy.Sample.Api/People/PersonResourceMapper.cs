using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Sample.Api.People;

public class PersonResourceMapper : IResourceMapper<Person, PersonResource>
{
    private readonly IRouteGenerator _routeGenerator;

    public PersonResourceMapper(IRouteGenerator routeGenerator)
    {
        _routeGenerator = routeGenerator;
    }
    
    public PersonResource Map(HttpContext httpContext, Person aggregateRoot)
    {
        Uri selfUri = _routeGenerator.Item(httpContext, aggregateRoot.Id);
        Link selfLink = Link.SelfLink(selfUri);
        
        PersonResource resource = new (aggregateRoot.Id, selfLink)
        {
            Forename = aggregateRoot.Forename,
            Surname = aggregateRoot.Surname,
            DateOfBirth = aggregateRoot.DateOfBirth,
            HomeAddressId = aggregateRoot.HomeAddressId
        };

        Uri homeAddressUri = _routeGenerator.Item(httpContext, aggregateRoot.HomeAddressId);
        Link homeAddressLink = Link.RelationLink(homeAddressUri);

        resource.Links.Add("homeAddress", homeAddressLink);
        
        return resource;
    }
}