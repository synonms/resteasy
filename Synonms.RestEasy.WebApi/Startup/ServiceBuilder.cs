using System.Reflection;
using Synonms.RestEasy.Core.Application;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Environment;
using Synonms.RestEasy.Core.Persistence;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Synonms.RestEasy.WebApi.Constants;
using Synonms.RestEasy.WebApi.Domain;
using Synonms.RestEasy.WebApi.MultiTenancy;
using Synonms.RestEasy.WebApi.MultiTenancy.Context;
using Synonms.RestEasy.WebApi.MultiTenancy.Resolution;
using Synonms.RestEasy.WebApi.Swashbuckle;

namespace Synonms.RestEasy.WebApi.Startup;

public class ServiceBuilder
{
    private readonly IServiceCollection _serviceCollection;
    
    public ServiceBuilder(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public ServiceBuilder WithCorsPolicy(Action<CorsPolicyBuilder> configurePolicy)
    {
        _serviceCollection.AddCors(corsOptions =>
        {
            corsOptions.AddPolicy(Cors.PolicyName, configurePolicy);
        });

        return this;
    }

    public ServiceBuilder WithApplicationDependenciesFrom(params Assembly[] assemblies)
    {
        _serviceCollection.RegisterAllImplementationsOf(typeof(IApplicationService), _serviceCollection.AddScoped, assemblies);
        
        return this;
    }

    public ServiceBuilder WithDomainDependenciesFrom(params Assembly[] assemblies)
    {
        _serviceCollection.RegisterAllImplementationsOf(typeof(IDomainService), _serviceCollection.AddScoped, assemblies);
        _serviceCollection.RegisterAllImplementationsOf(typeof(IDomainQuery), _serviceCollection.AddScoped, assemblies);
        _serviceCollection.RegisterAllImplementationsOf(typeof(IDomainCommand), _serviceCollection.AddScoped, assemblies);
        _serviceCollection.RegisterAllImplementationsOf(typeof(IAggregateRepository<>), _serviceCollection.AddScoped, assemblies);
        _serviceCollection.RegisterAllImplementationsOf(typeof(IAggregateCreator<,>), _serviceCollection.AddScoped, assemblies);
        _serviceCollection.RegisterAllImplementationsOf(typeof(IAggregateUpdater<,>), _serviceCollection.AddScoped, assemblies);

        return this;
    }
    
    public ServiceBuilder WithMultiTenancy<TTenant>(Assembly? additionalResolutionStrategiesAssembly = null, ServiceLifetime lifetime = ServiceLifetime.Transient) 
        where TTenant : Tenant
    {
        _serviceCollection.AddScoped<MultiTenancyMiddleware<TTenant>>();
        _serviceCollection.AddScoped<IMultiTenancyContextAccessor<TTenant>, MultiTenancyContextAccessor<TTenant>>();
        _serviceCollection.AddScoped<IMultiTenancyContextFactory<TTenant>, MultiTenancyContextFactory<TTenant>>();
        _serviceCollection.AddScoped<ITenantIdResolver, TenantIdResolver>();

        _serviceCollection.AddScoped<ITenantIdResolutionStrategy, HeaderTenantIdResolutionStrategy>();
        _serviceCollection.AddScoped<ITenantIdResolutionStrategy, QueryStringTenantIdResolutionStrategy>();

        if (additionalResolutionStrategiesAssembly is not null)
        {
            IEnumerable<Type> resolutionStrategyTypes = additionalResolutionStrategiesAssembly.GetTypes()
                .Where(x => !x.IsInterface && !x.IsAbstract && x.GetInterfaces().Contains(typeof(ITenantIdResolutionStrategy)));
            
            foreach (Type type in resolutionStrategyTypes)
            {
                _serviceCollection.Add(ServiceDescriptor.Describe(typeof(ITenantIdResolutionStrategy), type, lifetime));
            }
        }

        return this;
    }
    
    public ServiceBuilder WithOpenApi(Action<SwaggerGenOptions>? configurationAction = null)
    {
        _serviceCollection.AddEndpointsApiExplorer();
        _serviceCollection.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();
            options.DocumentFilter<RestEasyDocumentFilter>();

            configurationAction?.Invoke(options);
        });

        return this;
    }
    
    public ServiceBuilder WithLocalDateProvider(params Assembly[] assemblies)
    {
        _serviceCollection.AddSingleton<IDateProvider, LocalDateProvider>();
        
        return this;
    }

    public ServiceBuilder WithUtcDateProvider(params Assembly[] assemblies)
    {
        _serviceCollection.AddSingleton<IDateProvider, UtcDateProvider>();
        
        return this;
    }
}