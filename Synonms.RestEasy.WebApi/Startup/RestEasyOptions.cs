using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Synonms.RestEasy.WebApi.Startup;

public class RestEasyOptions
{
    public Assembly[] Assemblies { get; set; } = [];
    
    public Action<CorsPolicyBuilder>? CorsConfiguration { get; set; }
    
    public Assembly? AdditionalMultiTenancyResolutionStrategiesAssembly { get; set; }

    public Action<MvcOptions>? MvcOptionsConfigurationAction { get; set; }
    
    public Action<SwaggerGenOptions>? SwaggerGenConfigurationAction { get; set; }

    public string DefaultAuthenticationScheme { get; set; } = "Bearer";
    
    public Action<AuthenticationBuilder>? AuthenticationConfigurationAction { get; set; }
    
    public Action<AuthorizationOptions>? AuthorizationConfiguration { get; set; }
    
    public Action<SwaggerUIOptions>? SwaggerUiConfigurationAction { get; set; }
}