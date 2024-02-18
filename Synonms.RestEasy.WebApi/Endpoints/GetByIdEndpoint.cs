using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.WebApi.Http;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Schema.Resources;
using Synonms.RestEasy.WebApi.Mediation.Queries;

namespace Synonms.RestEasy.WebApi.Endpoints;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
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
        EntityTag? ifNoneMatch = HttpContext.Request.Headers.ExtractIfNoneMatch();
        
        FindResourceRequest<TAggregateRoot, TResource> request = new(id)
        {
            IfNoneMatch = ifNoneMatch
        };
        
        FindResourceResponse<TAggregateRoot, TResource> response = await _mediator.Send(request);
    
        return response.Outcome
            .Match<IActionResult>(resource =>
                {
                    ResourceDocument<TResource> document = new(resource.SelfLink, resource);

                    HttpContext.Response.Headers[HeaderNames.ETag] = response.EntityTag.ToString();     

                    return Ok(document);
                },
                () => StatusCode(StatusCodes.Status404NotFound));
    }
}