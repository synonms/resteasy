using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Mediation.Queries;

namespace Synonms.RestEasy.Endpoints;

[ApiController]
public class GetByIdEndpoint<TAggregateRoot, TResource> : ControllerBase
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    private readonly IMediator _mediator;
    private readonly IResourceMapper<TAggregateRoot, TResource> _resourceMapper;

    public GetByIdEndpoint(IMediator mediator, IResourceMapper<TAggregateRoot, TResource> resourceMapper)
    {
        _mediator = mediator;
        _resourceMapper = resourceMapper;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] EntityId<TAggregateRoot> id)
    {
        // TODO: Support parameters
        FindResourceRequest<TAggregateRoot, TResource> request = new(HttpContext, id);
        FindResourceResponse<TAggregateRoot, TResource> response = await _mediator.Send(request);
    
        return response.Outcome
            .Match<IActionResult>(resource =>
                {
                    ResourceDocument<TAggregateRoot, TResource> document = new(resource.SelfLink, resource);

                    return Ok(document);
                },
                () => StatusCode(StatusCodes.Status404NotFound));
    }
}