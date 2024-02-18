using Synonms.RestEasy.Core.Domain;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Schema.Forms;
using Synonms.RestEasy.Core.Schema.Resources;
using Synonms.RestEasy.WebApi.Mediation.Queries;
using Synonms.RestEasy.WebApi.Routing;

namespace Synonms.RestEasy.WebApi.Endpoints;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[EnableCors(Cors.PolicyName)]
public class EditFormEndpoint<TAggregateRoot, TResource> : ControllerBase
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource, new()
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
        FindResourceRequest<TAggregateRoot, TResource> request = new(id);
        FindResourceResponse<TAggregateRoot, TResource> response = await _mediator.Send(request);

        return response.Outcome
            .Match<IActionResult>(resource =>
                {
                    Uri editFormUri = _routeGenerator.EditForm<TAggregateRoot>(id);
                    Uri targetUri = _routeGenerator.Item<TAggregateRoot>(id);
                    FormDocument document = _documentFactory.Create(editFormUri, targetUri, resource);

                    return Ok(document);
                },
                () => StatusCode(StatusCodes.Status404NotFound));
    }
}