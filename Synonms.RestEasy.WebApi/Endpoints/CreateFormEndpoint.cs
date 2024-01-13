using Synonms.RestEasy.Core.Domain;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Routing;
using Synonms.RestEasy.WebApi.Schema.Forms;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Endpoints;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[EnableCors(Cors.PolicyName)]
public class CreateFormEndpoint<TAggregateRoot, TResource> : ControllerBase
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource, new()
{
    private readonly IRouteGenerator _routeGenerator;
    private readonly ICreateFormDocumentFactory<TAggregateRoot, TResource> _documentFactory;

    public CreateFormEndpoint(IRouteGenerator routeGenerator, ICreateFormDocumentFactory<TAggregateRoot, TResource> documentFactory)
    {
        _routeGenerator = routeGenerator;
        _documentFactory = documentFactory;
    }
    
    [HttpGet]
    [Route(IanaLinkRelations.Forms.Create)]
    public Task<IActionResult> CreateFormAsync()
    {
        Uri createFormUri = _routeGenerator.CreateForm<TAggregateRoot>();
        Uri targetUri = _routeGenerator.Collection<TAggregateRoot>();
        TResource resource = new();
        FormDocument document = _documentFactory.Create(createFormUri, targetUri, resource);

        return Task.FromResult(Ok(document) as IActionResult);
    }
}