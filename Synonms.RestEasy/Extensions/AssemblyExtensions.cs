using System.Reflection;

namespace Synonms.RestEasy.Extensions;

public static class AssemblyExtensions
{
    public static IEnumerable<Type> GetAggregateRoots(this Assembly assembly) =>
        assembly.GetTypes().Where(x => x.IsAggregateRoot());

    public static IEnumerable<Type> GetResources(this Assembly assembly) =>
        assembly.GetTypes().Where(x => x.IsResource());
}