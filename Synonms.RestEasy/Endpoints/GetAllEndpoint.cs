using MediatR;
using Microsoft.AspNetCore.Mvc;
using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Extensions;
using Synonms.RestEasy.Mediation.Queries;

namespace Synonms.RestEasy.Endpoints;

[ApiController]
public class GetAllEndpoint<TAggregateRoot, TResource> : ControllerBase
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    private readonly IMediator _mediator;
    private readonly IResourceMapper<TAggregateRoot, TResource> _resourceMapper;
    private readonly IRouteGenerator _routeGenerator;

    public GetAllEndpoint(IMediator mediator, IResourceMapper<TAggregateRoot, TResource> resourceMapper, IRouteGenerator routeGenerator)
    {
        _mediator = mediator;
        _resourceMapper = resourceMapper;
        _routeGenerator = routeGenerator;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllAsync([FromQuery] int offset = 0)
    {
        // TODO: Customisable parameters
        ReadResourceCollectionRequest<TAggregateRoot, TResource> request = new(HttpContext, offset, 25);
        ReadResourceCollectionResponse<TAggregateRoot, TResource> response = await _mediator.Send(request);

        Uri selfUri = _routeGenerator.Collection<TAggregateRoot>(HttpContext, offset);
        Link selfLink = Link.SelfLink(selfUri);
        
        Pagination pagination = response.ResourceCollection.GeneratePagination(o => _routeGenerator.Collection<TAggregateRoot>(HttpContext, o));

        ResourceCollectionDocument<TAggregateRoot, TResource> document = new(selfLink, response.ResourceCollection, pagination);

        return Ok(document);
    }
}