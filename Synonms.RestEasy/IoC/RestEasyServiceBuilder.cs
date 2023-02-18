using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.SharedKernel.Extensions;

namespace Synonms.RestEasy.IoC;

public class RestEasyServiceBuilder
{
    private readonly IServiceCollection _serviceCollection;
    
    public RestEasyServiceBuilder(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public RestEasyServiceBuilder WithRepositoriesFrom(params Assembly[] assemblies)
    {
        _serviceCollection.RegisterAllImplementationsOf(typeof(ICreateRepository<>), _serviceCollection.AddSingleton, assemblies);
        _serviceCollection.RegisterAllImplementationsOf(typeof(IReadRepository<>), _serviceCollection.AddSingleton, assemblies);
        _serviceCollection.RegisterAllImplementationsOf(typeof(IUpdateRepository<>), _serviceCollection.AddSingleton, assemblies);
        _serviceCollection.RegisterAllImplementationsOf(typeof(IDeleteRepository<>), _serviceCollection.AddSingleton, assemblies);
        _serviceCollection.RegisterAllImplementationsOf(typeof(IAggregateCreator<,>), _serviceCollection.AddSingleton, assemblies);
        _serviceCollection.RegisterAllImplementationsOf(typeof(IAggregateUpdater<,>), _serviceCollection.AddSingleton, assemblies);

        return this;
    }

}