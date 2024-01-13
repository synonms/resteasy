using Microsoft.AspNetCore.Builder;
using Synonms.RestEasy.WebApi.Correlation;

namespace Synonms.RestEasy.WebApi.Startup;

public static class WebApplicationExtensions
{
    public static PipelineBuilder UseRestEasyFramework(this WebApplication webApplication)
    {
        webApplication.UseMiddleware<CorrelationMiddleware>();

        return new PipelineBuilder(webApplication);
    }
}