using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Synonms.RestEasy.Core.Constants;
using Synonms.RestEasy.WebApi.Auth;
using Synonms.RestEasy.WebApi.Correlation;
using Synonms.RestEasy.WebApi.Http;
using Synonms.RestEasy.WebApi.MultiTenancy;

namespace Synonms.RestEasy.WebApi.Startup;

public static class WebApplicationExtensions
{
    public static WebApplication UseMultiTenantRestEasyFramework<TTenant>(this WebApplication webApplication, RestEasyOptions options)
        where TTenant : Tenant =>
        webApplication
            .PreMultiTenancy(options)
            .UseMultiTenancy<TTenant>()
            .PostMultiTenancy(options);

    public static WebApplication UseRestEasyFramework(this WebApplication webApplication, RestEasyOptions options) =>
        webApplication
            .PreMultiTenancy(options)
            .PostMultiTenancy(options);
    
    private static WebApplication PreMultiTenancy(this WebApplication webApplication, RestEasyOptions options)
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
        webApplication.UseRouting();
        webApplication.UseCors(Cors.PolicyName);

        return webApplication;
    }

    private static WebApplication UseMultiTenancy<TTenant>(this WebApplication webApplication)
        where TTenant : Tenant
    {
        webApplication.UseMiddleware<MultiTenancyMiddleware<TTenant>>();

        return webApplication;
    }

    private static WebApplication PostMultiTenancy(this WebApplication webApplication, RestEasyOptions options)
    {
        webApplication.UseAuthentication();
        if (options.AppendPermissions)
        {
            webApplication.UseMiddleware<PermissionsMiddleware>();
        }
        webApplication.UseAuthorization();
        
        webApplication.MapControllers();

        return webApplication;
    }
}