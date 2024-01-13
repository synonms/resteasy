using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerUI;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.MultiTenancy;

namespace Synonms.RestEasy.WebApi.Startup;

public class PipelineBuilder
{
    private readonly WebApplication _webApplication;

    public PipelineBuilder(WebApplication webApplication)
    {
        _webApplication = webApplication;
    }
    
    public PipelineBuilder WithOpenApi(Action<SwaggerUIOptions> setupAction)
    {
        if (_webApplication.Environment.IsDevelopment())
        {
            _webApplication.UseSwagger(options =>
            {
                options.RouteTemplate = "/swagger/{documentName}/swagger.json";
            });
            _webApplication.UseSwaggerUI(setupAction.Invoke);
        }

        return this;
    }

    public PipelineBuilder WithRouting()
    {
        _webApplication.UseRouting();

        return this;
    }
    
    public PipelineBuilder WithCors()
    {
        _webApplication.UseCors(Cors.PolicyName);

        return this;
    }

    public PipelineBuilder WithAuthentication()
    {
        _webApplication.UseAuthentication();

        return this;
    }
    
    public PipelineBuilder WithAuthorization()
    {
        _webApplication.UseAuthorization();

        return this;
    }

    public PipelineBuilder WithMultiTenancy<TTenant>() where TTenant : Tenant
    {
        _webApplication.UseMiddleware<MultiTenancyMiddleware<TTenant>>();

        return this;
    }
    
    public PipelineBuilder WithControllers()
    {
        _webApplication.MapControllers();

        return this;
    }
}