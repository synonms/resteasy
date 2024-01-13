using System;
using System.Collections.Generic;
using System.Linq;
using Synonms.RestEasy.Core.Extensions;
using Xunit;

namespace Synonms.RestEasy.WebApi.Tests.Unit.Runtime;

internal interface ISomeUnusedNonGenericInterface {}
internal interface ISomeNonGenericInterface {}
internal interface ISomeHierarchicalNonGenericInterface : ISomeNonGenericInterface {}
internal class SomeNonGenericImplementation : ISomeNonGenericInterface {}
internal class SomeHierarchicalNonGenericImplementation : ISomeHierarchicalNonGenericInterface {}
internal interface ISomeUnusedGenericInterface<T> {}
internal interface ISomeGenericInterface<T> {}
internal class SomeIntImplementation : ISomeGenericInterface<int> {}
internal class SomeStringImplementation : ISomeGenericInterface<string> {}

public class AssemblyExtensionsTests
{
    [Fact]
    public void GetImplementationsOfGenericInterface_NoImplementations_ReturnsNoTypes()
    {
        List<Type> types = typeof(AssemblyExtensionsTests).Assembly
            .GetImplementationsOfGenericInterface(typeof(ISomeUnusedGenericInterface<>))
            .ToList();
        
        Assert.Empty(types);
    }

    [Fact]
    public void GetImplementationsOfGenericInterface_SomeImplementations_ReturnsAllTypes()
    {
        List<Type> types = typeof(AssemblyExtensionsTests).Assembly
            .GetImplementationsOfGenericInterface(typeof(ISomeGenericInterface<>))
            .ToList();
        
        Assert.Equal(2, types.Count);
        Assert.Contains(types, x => x == typeof(SomeIntImplementation));
        Assert.Contains(types, x => x == typeof(SomeStringImplementation));
    }
    
    [Fact]
    public void GetImplementationsOfNonGenericInterface_NoImplementations_ReturnsNoTypes()
    {
        List<Type> types = typeof(AssemblyExtensionsTests).Assembly
            .GetImplementationsOfNonGenericInterface(typeof(ISomeUnusedNonGenericInterface))
            .ToList();
        
        Assert.Empty(types);
    }

    [Fact]
    public void GetImplementationsOfNonGenericInterface_SomeImplementations_ReturnsAllTypes()
    {
        List<Type> types = typeof(AssemblyExtensionsTests).Assembly
            .GetImplementationsOfNonGenericInterface(typeof(ISomeNonGenericInterface))
            .ToList();
        
        Assert.Equal(2, types.Count);
        Assert.Contains(types, x => x == typeof(SomeNonGenericImplementation));
        Assert.Contains(types, x => x == typeof(SomeHierarchicalNonGenericImplementation));
    }
}