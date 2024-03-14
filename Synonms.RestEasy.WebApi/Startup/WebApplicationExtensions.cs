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
        webApplication.UseRouting();
        webApplication.UseCors(Cors.PolicyName);

        webApplication.UseAuthentication();
        
        webApplication.UseMiddleware<UserMiddleware<TUser>>();
        webApplication.UseMiddleware<TenantMiddleware<TUser, TTenant>>();
        webApplication.UseMiddleware<ProductMiddleware<TUser, TProduct>>();
        webApplication.UseMiddleware<PermissionsMiddleware<TUser, TProduct, TTenant>>();

        webApplication.UseAuthorization();
        
        webApplication.MapControllers();

        return webApplication;
    }
}