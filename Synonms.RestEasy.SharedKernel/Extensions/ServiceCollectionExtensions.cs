using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Synonms.RestEasy.SharedKernel.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAllImplementationsOf(this IServiceCollection serviceCollection, Type interfaceType, Func<Type, Type, IServiceCollection> registrationFunc, params Assembly[] assemblies)
    {
        if (interfaceType.IsGenericType)
        {
            assemblies
                .SelectMany(assembly => assembly.GetImplementationsOfGenericInterface(interfaceType))
                .ToList()
                .ForEach(implementationType =>
                {
                    List<Type> implementedInterfaces = implementationType.GetInterfaces().ToList();
                    implementedInterfaces.ForEach(implementedInterface =>
                    {
                        registrationFunc(implementedInterface, implementationType);
                    });
                });
        }
        else
        {
            assemblies
                .SelectMany(assembly => assembly.GetImplementationsOfNonGenericInterface(interfaceType))
                .ToList()
                .ForEach(implementationType =>
                {
                    List<Type> implementedInterfaces = implementationType.GetInterfaces().ToList();
                    implementedInterfaces.ForEach(implementedInterface =>
                    {
                        registrationFunc(implementedInterface, implementationType);
                    });
                });
        }

        return serviceCollection;
    }
}