using System.Reflection;
using System.Text.RegularExpressions;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.WebApi.Collections;
using Synonms.RestEasy.WebApi.Pipeline;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Synonms.RestEasy.Core.Attributes;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.Core.Schema;
using Synonms.RestEasy.Core.Schema.Resources;
using Synonms.RestEasy.WebApi.Mediation.Queries;
using Synonms.RestEasy.WebApi.Routing;

namespace Synonms.RestEasy.WebApi.Endpoints;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[EnableCors(Cors.PolicyName)]
public class GetAllEndpoint<TAggregateRoot, TResource> : ControllerBase
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    private readonly IMediator _mediator;
    private readonly IRouteGenerator _routeGenerator;

    public GetAllEndpoint(IMediator mediator, IRouteGenerator routeGenerator)
    {
        _mediator = mediator;
        _routeGenerator = routeGenerator;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllAsync([FromQuery] int offset = 0, [FromQuery] int limit = Pagination.DefaultPageLimit)
    {
        RestEasyResourceAttribute? resourceAttribute = typeof(TAggregateRoot).GetCustomAttribute<RestEasyResourceAttribute>();

        int configuredPageLimit = resourceAttribute?.PageLimit ?? 0;
        int pageLimit = configuredPageLimit > 0 ? configuredPageLimit : Math.Clamp(limit, 0, int.MaxValue);
        
        ReadResourceCollectionRequest<TAggregateRoot, TResource> request = new(pageLimit)
        {
            Offset = offset,
            QueryParameters = Request.Query.ExtractQueryParameters<TAggregateRoot>(),
            SortItems = Request.Query.ExtractSortItems()
        };
        ReadResourceCollectionResponse<TAggregateRoot, TResource> response = await _mediator.Send(request);

        Uri selfUri = _routeGenerator.Collection<TAggregateRoot>(request.QueryParameters);
        Link selfLink = Link.SelfLink(selfUri);
        
        Pagination pagination = response.ResourceCollection.GeneratePagination(o =>
            _routeGenerator.Collection<TAggregateRoot>(request.QueryParameters)
        );

        ResourceCollectionDocument<TResource> document = new(selfLink, response.ResourceCollection, pagination);

        if (resourceAttribute?.IsCreateDisabled is false)
        {
            Uri createFormUri = _routeGenerator.CreateForm<TAggregateRoot>();
            Link createFormLink = Link.CreateFormLink(createFormUri);
            document.WithLink(IanaLinkRelations.Forms.Create, createFormLink);
        }

        return Ok(document);
    }
}