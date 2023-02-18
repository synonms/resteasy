using System.Reflection;

namespace Synonms.RestEasy.SharedKernel.Extensions;

public static class AssemblyExtensions
{
    public static IEnumerable<Type> GetImplementationsOfGenericInterface(this Assembly assembly, Type genericInterfaceType) =>
        assembly.GetTypes()
            .Where(item => item.GetInterfaces()
                               .Where(i => i.IsGenericType)
                               .Any(i => i.GetGenericTypeDefinition() == genericInterfaceType)
                           && !item.IsAbstract
                           && !item.IsInterface);
    
    public static IEnumerable<Type> GetImplementationsOfNonGenericInterface(this Assembly assembly, Type nonGenericInterfaceType) =>
        assembly.GetTypes()
            .Where(item => item.GetInterfaces()
                               .Where(i => !i.IsGenericType)
                               .Any(i => i == nonGenericInterfaceType)
                           && !item.IsAbstract
                           && !item.IsInterface);
}