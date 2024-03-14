﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Synonms.RestEasy.WebApi.Http;

namespace Synonms.RestEasy.WebApi.Correlation;

public class RestEasyCorrelationRelayHandler : DelegatingHandler
{
    private readonly ILogger<RestEasyCorrelationRelayHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RestEasyCorrelationRelayHandler(ILogger<RestEasyCorrelationRelayHandler> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Relaying Correlation...");
        
        HttpContext? httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            _logger.LogDebug("HTTP Context not found - skipping correlation relay.");
        }
        else
        {
            if (httpContext.Items.TryGetValue(HttpContextItemKeys.CorrelationId, out object? correlationItem) && correlationItem is Guid correlationId)
            {
                _logger.LogDebug("Correlation Id found - adding to request.");
                request.Headers.Add(HttpHeaders.CorrelationId, correlationId.ToString());
            }
            else
            {
                _logger.LogDebug("Correlation Id not found - skipping correlation relay.");
            }
        }

        _logger.LogDebug("Correlation relay complete.");
        
        return await base.SendAsync(request, cancellationToken);
    }
}
