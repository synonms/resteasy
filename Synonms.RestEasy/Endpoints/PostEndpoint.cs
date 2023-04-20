using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Abstractions.Schema.Errors;
using Synonms.RestEasy.Application.Faults;
using Synonms.RestEasy.Domain.Faults;
using Synonms.RestEasy.Mediation.Commands;

namespace Synonms.RestEasy.Endpoints;

[ApiController]
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
        // TODO: Support parameters
        CreateResourceRequest<TAggregateRoot, TResource> request = new(resource);
        CreateResourceResponse<TAggregateRoot> response = await _mediator.Send(request);

        return response.Outcome
            .Match(
                aggregateRoot =>
                {
                    Response.Headers[HeaderNames.Location] = _routeGenerator.Item(HttpContext, aggregateRoot.Id).OriginalString;
        
                    return StatusCode(StatusCodes.Status201Created);
                },
                fault =>
                {
                    Uri collectionUri = _routeGenerator.Collection<TAggregateRoot>(HttpContext);
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
