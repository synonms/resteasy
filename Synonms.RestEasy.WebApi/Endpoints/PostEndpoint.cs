using Synonms.RestEasy.Core.Application.Faults;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.WebApi.Http;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Schema;
using Synonms.RestEasy.Core.Schema.Errors;
using Synonms.RestEasy.Core.Schema.Resources;
using Synonms.RestEasy.WebApi.Mediation.Commands;
using Synonms.RestEasy.WebApi.Routing;

namespace Synonms.RestEasy.WebApi.Endpoints;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[EnableCors(Cors.PolicyName)]
public class PostEndpoint<TAggregateRoot, TResource> : ControllerBase
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    private readonly IMediator _mediator;
    private readonly IRouteGenerator _routeGenerator;
    private readonly IErrorCollectionDocumentFactory _errorCollectionDocumentFactory;

    public PostEndpoint(IMediator mediator, IRouteGenerator routeGenerator, IErrorCollectionDocumentFactory errorCollectionDocumentFactory)
    {
        _mediator = mediator;
        _routeGenerator = routeGenerator;
        _errorCollectionDocumentFactory = errorCollectionDocumentFactory;
    }
    
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> PostAsync([FromBody] TResource resource)
    {
        CreateResourceRequest<TAggregateRoot, TResource> request = new(resource);
        CreateResourceResponse<TAggregateRoot> response = await _mediator.Send(request);

        return response.Outcome
            .Match(
                aggregateRoot =>
                {
                    Response.Headers[HeaderNames.Location] = _routeGenerator.Item(aggregateRoot.Id).OriginalString;
                    Response.Headers[HeaderNames.ETag] = aggregateRoot.EntityTag.ToString();    

                    return StatusCode(StatusCodes.Status201Created);
                },
                fault =>
                {
                    Uri collectionUri = _routeGenerator.Collection<TAggregateRoot>();
                    Link requestedDocumentLink = new(collectionUri, IanaLinkRelations.Collection, IanaHttpMethods.Post);
                    ErrorCollectionDocument document = _errorCollectionDocumentFactory.Create(fault, requestedDocumentLink);

                    // TODO: Return error document for all 4xx status codes
                    IActionResult result = fault switch
                    {
                        ApplicationRulesFault applicationRulesFault => BadRequest(document),
                        ApplicationRuleFault applicationRuleFault => BadRequest(document),
                        DomainRulesFault domainRulesFault => BadRequest(document),
                        DomainRuleFault domainRuleFault => BadRequest(document),
                        EntityNotFoundFault entityNotFoundFault => NotFound(document),
                        _ => StatusCode(StatusCodes.Status500InternalServerError) 
                    };
                    
                    return result;
                });
    }
}