using System.Reflection;
using Synonms.RestEasy.Core.Extensions;

namespace Synonms.RestEasy.Core.Runtime;

public static class AssemblyExtensions
{
    public static IEnumerable<Type> GetAggregateRoots(this Assembly assembly) =>
        assembly.GetTypes().Where(x => x.IsAggregateRoot());

    public static IEnumerable<Type> GetAggregateMembers(this Assembly assembly) =>
        assembly.GetTypes().Where(x => x.IsAggregateMember());

    public static IEnumerable<Type> GetResources(this Assembly assembly) =>
        assembly.GetTypes().Where(x => x.IsResource());
    
    public static IEnumerable<Type> GetChildResources(this Assembly assembly) =>
        assembly.GetTypes().Where(x => x.IsChildResource());
}