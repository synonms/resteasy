using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.WebApi.Auth;
using Synonms.RestEasy.WebApi.Correlation;
using Synonms.RestEasy.WebApi.Http;
using Synonms.RestEasy.WebApi.Pipeline.Products;
using Synonms.RestEasy.WebApi.Pipeline.Tenants;
using Synonms.RestEasy.WebApi.Pipeline.Users;

namespace Synonms.RestEasy.WebApi.Startup;

public static class WebApplicationExtensions
{
    public static WebApplication UseRestEasyFramework<TUser, TProduct, TTenant>(this WebApplication webApplication, RestEasyOptions options)
        where TUser : RestEasyUser
        where TProduct : RestEasyProduct
        where TTenant : RestEasyTenant
    {
        webApplication.UseHttpsRedirection();

        webApplication.UseMiddleware<OptionsMiddleware>();

        if (webApplication.Environment.IsDevelopment())
        {
            webApplication.UseSwagger(swaggerOptions =>
            {
                swaggerOptions.RouteTemplate = "/swagger/{documentName}/swagger.json";
            });
            webApplication.UseSwaggerUI(options.SwaggerUiConfigurationAction);
        }

        webApplication.UseMiddleware<CorrelationMiddleware>();
        
        options.PreRoutingPipelineConfigurationAction?.Invoke(webApplication);
        webApplication.UseRouting();
        options.PostRoutingPipelineConfigurationAction?.Invoke(webApplication);
        
        webApplication.UseCors(Cors.PolicyName);

        options.PreAuthenticationPipelineConfigurationAction?.Invoke(webApplication);
        webApplication.UseAuthentication();
        options.PostAuthenticationPipelineConfigurationAction?.Invoke(webApplication);
        
        webApplication.UseMiddleware<UserMiddleware<TUser>>();
        webApplication.UseMiddleware<TenantMiddleware<TUser, TTenant>>();
        webApplication.UseMiddleware<ProductMiddleware<TUser, TProduct>>();
        webApplication.UseMiddleware<PermissionsMiddleware<TUser, TProduct, TTenant>>();

        options.PreAuthorizationPipelineConfigurationAction?.Invoke(webApplication);
        webApplication.UseAuthorization();
        options.PostAuthorizationPipelineConfigurationAction?.Invoke(webApplication);
        
        ControllerActionEndpointConventionBuilder controllerActionEndpointConventionBuilder = webApplication.MapControllers();

        options.ControllerActionConfigurationAction?.Invoke(controllerActionEndpointConventionBuilder);
        
        return webApplication;
    }
}