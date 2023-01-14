using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Synonms.RestEasy.IoC;

public class RestEasyServiceBuilder
{
    private readonly IServiceCollection _serviceCollection;
    
    public RestEasyServiceBuilder(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public RestEasyServiceBuilder WithAggregatesFrom(params Assembly[] assemblies)
    {
        
        return this;
    }

}