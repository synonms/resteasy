using MediatR;
using Microsoft.AspNetCore.Mvc;
using Synonms.RestEasy.Abstractions.Constants;
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
    private readonly IRouteGenerator _routeGenerator;

    public GetAllEndpoint(IMediator mediator, IRouteGenerator routeGenerator)
    {
        _mediator = mediator;
        _routeGenerator = routeGenerator;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllAsync([FromQuery] int offset = 0)
    {
        // TODO: Customisable parameters
        // TODO: Customisable page limit
        ReadResourceCollectionRequest<TAggregateRoot, TResource> request = new(HttpContext, offset, 5);
        ReadResourceCollectionResponse<TAggregateRoot, TResource> response = await _mediator.Send(request);

        // TODO: Append query String as it may contain parentId filters
        Uri selfUri = _routeGenerator.Collection<TAggregateRoot>(HttpContext, offset);
        Link selfLink = Link.SelfLink(selfUri);
        Uri createFormUri = _routeGenerator.CreateForm<TAggregateRoot>(HttpContext);
        Link createFormLink = Link.CreateFormLink(createFormUri);
        
        Pagination pagination = response.ResourceCollection.GeneratePagination(o => _routeGenerator.Collection<TAggregateRoot>(HttpContext, o));

        ResourceCollectionDocument<TAggregateRoot, TResource> document = new(selfLink, response.ResourceCollection, pagination);

        document.WithLink(IanaLinkRelations.Forms.Create, createFormLink);
            
        return Ok(document);
    }
}