using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Synonms.RestEasy.Abstractions.Constants;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Routing;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Documents;
using Synonms.RestEasy.Extensions;
using Synonms.RestEasy.Mediation.Queries;

namespace Synonms.RestEasy.Endpoints;

[ApiController]
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
    public async Task<IActionResult> GetAllAsync([FromQuery] int offset = 0)
    {
        IReadOnlyDictionary<string, object>? parameters = PrepareParameters(Request.Query);
        
        // TODO: Customisable page limit
        ReadResourceCollectionRequest<TAggregateRoot, TResource> request = new(HttpContext, offset, 5, parameters);
        ReadResourceCollectionResponse<TAggregateRoot, TResource> response = await _mediator.Send(request);

        Uri selfUri = _routeGenerator.Collection<TAggregateRoot>(HttpContext, Request.Query.Any() ? Request.Query : null);
        Link selfLink = Link.SelfLink(selfUri);
        Uri createFormUri = _routeGenerator.CreateForm<TAggregateRoot>(HttpContext);
        Link createFormLink = Link.CreateFormLink(createFormUri);
        
        Pagination pagination = response.ResourceCollection.GeneratePagination(o =>
        {
            Dictionary<string, StringValues> query = Request.Query.ToDictionary(x => x.Key, x => x.Value);
            query["offset"] = o.ToString();
            QueryCollection queryCollection = new(query);
            
            return _routeGenerator.Collection<TAggregateRoot>(HttpContext, queryCollection);
        });

        ResourceCollectionDocument<TResource> document = new(selfLink, response.ResourceCollection, pagination);

        document.WithLink(IanaLinkRelations.Forms.Create, createFormLink);
            
        return Ok(document);
    }

    // TODO: Move this somewhere shared
    private IReadOnlyDictionary<string, object>? PrepareParameters(IQueryCollection query)
    {
        Dictionary<string, object> dictionary = new();
        
        foreach ((string key, StringValues value) in query)
        {
            PropertyInfo? aggregatePropertyInfo = typeof(TAggregateRoot).GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            
            if (aggregatePropertyInfo?.PropertyType.IsEntityId() ?? false)
            {
                if (Guid.TryParse(value.ToString(), out Guid guid))
                {
                    ConstructorInfo? entityIdConstructor = aggregatePropertyInfo?.PropertyType.GetConstructor(new Type[] { typeof(Guid) });

                    if (entityIdConstructor is not null)
                    {
                        object entityId = entityIdConstructor.Invoke(new object?[] { guid });
                        dictionary[key] = entityId;
                        continue;
                    }
                    
                    dictionary[key] = guid;
                }
            }
            
            // TODO: Convert any other non-string types

            dictionary[key] = value.ToString();
        }

        return dictionary;
    }
}