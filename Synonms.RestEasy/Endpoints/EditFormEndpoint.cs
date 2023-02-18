using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Mediation.Queries;

namespace Synonms.RestEasy.Endpoints;

[ApiController]
public class EditFormEndpoint<TAggregateRoot, TResource> : ControllerBase
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>, new()
{
    private readonly IMediator _mediator;
    private readonly IRouteGenerator _routeGenerator;
    private readonly IEditFormDocumentFactory<TAggregateRoot, TResource> _documentFactory;

    public EditFormEndpoint(IMediator mediator, IRouteGenerator routeGenerator, IEditFormDocumentFactory<TAggregateRoot, TResource> documentFactory)
    {
        _mediator = mediator;
        _routeGenerator = routeGenerator;
        _documentFactory = documentFactory;
    }
    
    [HttpGet]
    [Route("{id}/" + IanaLinkRelations.Forms.Edit)]
    public async Task<IActionResult> EditFormAsync([FromRoute] EntityId<TAggregateRoot> id)
    {
        // TODO: Support parameters
        FindResourceRequest<TAggregateRoot, TResource> request = new(HttpContext, id);
        FindResourceResponse<TAggregateRoot, TResource> response = await _mediator.Send(request);

        return response.Outcome
            .Match<IActionResult>(resource =>
                {
                    Uri editFormUri = _routeGenerator.EditForm<TAggregateRoot>(HttpContext, id);
                    Uri targetUri = _routeGenerator.Item<TAggregateRoot>(HttpContext, id);
                    FormDocument document = _documentFactory.Create(editFormUri, targetUri, resource);

                    return Ok(document);
                },
                () => StatusCode(StatusCodes.Status404NotFound));
    }
}