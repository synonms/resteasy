using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Abstractions.Schema.Forms;

namespace Synonms.RestEasy.Endpoints;

[ApiController]
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
        Uri createFormUri = _routeGenerator.CreateForm<TAggregateRoot>(HttpContext);
        Uri targetUri = _routeGenerator.Collection<TAggregateRoot>(HttpContext);
        TResource resource = new();
        FormDocument document = _documentFactory.Create(createFormUri, targetUri, resource);

        return Task.FromResult(Ok(document) as IActionResult);
    }
}