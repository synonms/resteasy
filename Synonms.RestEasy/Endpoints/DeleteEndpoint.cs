using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Mediation.Commands;

namespace Synonms.RestEasy.Endpoints;

[ApiController]
[EnableCors(Cors.PolicyName)]
public class DeleteEndpoint<TAggregateRoot> : ControllerBase
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    private readonly IMediator _mediator;

    public DeleteEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] EntityId<TAggregateRoot> id)
    {
        // TODO: Support parameters
        DeleteResourceRequest<TAggregateRoot> request = new(id);
        DeleteResourceResponse response = await _mediator.Send(request);

        return response.Outcome.Match(
            fault => NotFound(),
            () => StatusCode(StatusCodes.Status204NoContent));
    }
}