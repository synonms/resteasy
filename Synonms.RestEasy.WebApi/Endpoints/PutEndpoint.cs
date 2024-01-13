using Synonms.RestEasy.Core.Application.Faults;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.WebApi.Http;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Mediation.Commands;
using Synonms.RestEasy.WebApi.Routing;
using Synonms.RestEasy.WebApi.Schema;
using Synonms.RestEasy.WebApi.Schema.Errors;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Endpoints;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[EnableCors(Cors.PolicyName)]
public class PutEndpoint<TAggregateRoot, TResource> : ControllerBase
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    private readonly IMediator _mediator;
    private readonly IRouteGenerator _routeGenerator;
    private readonly IErrorCollectionDocumentFactory _errorCollectionDocumentFactory;

    public PutEndpoint(IMediator mediator, IRouteGenerator routeGenerator, IErrorCollectionDocumentFactory errorCollectionDocumentFactory)
    {
        _mediator = mediator;
        _routeGenerator = routeGenerator;
        _errorCollectionDocumentFactory = errorCollectionDocumentFactory;
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> PutAsync([FromRoute] EntityId<TAggregateRoot> id, [FromBody] TResource resource)
    {
        // The Id should not be passed in the body so the resource.Id value will be a random Guid.
        // Align it so that if resource.Id is accessed during the request pipeline it is correct as per the route. 
        resource.Id = id.Value;
        
        EntityTag? ifMatch = HttpContext.Request.Headers.ExtractIfMatch();

        UpdateResourceRequest<TAggregateRoot, TResource> request = new(id, resource)
        {
            IfMatch = ifMatch
        };
        
        UpdateResourceResponse<TAggregateRoot> response = await _mediator.Send(request);
    
        return response.Outcome
            .Match(
                aggregateRoot =>
                {
                    Response.Headers[HeaderNames.ETag] = aggregateRoot.EntityTag.ToString();    

                    return StatusCode(StatusCodes.Status204NoContent);
                },
                fault =>
                {
                    Uri itemUri = _routeGenerator.Item(id);
                    Link requestedDocumentLink = new (itemUri, IanaLinkRelations.Item, IanaHttpMethods.Put);
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