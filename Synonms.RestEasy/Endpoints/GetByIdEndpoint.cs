using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Mediation.Queries;

namespace Synonms.RestEasy.Endpoints;

[ApiController]
[EnableCors(Cors.PolicyName)]
public class GetByIdEndpoint<TAggregateRoot, TResource> : ControllerBase
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    private readonly IMediator _mediator;

    public GetByIdEndpoint(IMediator mediator)
    {
        _mediator = mediator;
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
                    ResourceDocument<TResource> document = new(resource.SelfLink, resource);

                    return Ok(document);
                },
                () => StatusCode(StatusCodes.Status404NotFound));
    }
}