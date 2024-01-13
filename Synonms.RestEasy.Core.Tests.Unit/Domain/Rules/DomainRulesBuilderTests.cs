using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules;
using Xunit;

namespace Synonms.RestEasy.Core.Tests.Unit.Domain.Rules;

public class DomainRulesBuilderTests
{
    [Fact]
    public void Build_AllFaults_ReturnsAllFaults()
    {
        const string faultDetailA = "faultA";
        const string faultDetailB1 = "faultB1";
        const string faultDetailB2 = "faultB2";
        const string faultDetailC = "faultC";
        
        IEnumerable<DomainRuleFault> faults = DomainRules.CreateBuilder()
            .Property<string>("SomeStringProperty", "test").AddRule(_ => true, faultDetailA)
            .Property<string?>("SomeOptionalStringProperty", null).AddRule(_ => true, faultDetailB1).AddRule(_ => true, faultDetailB2)
            .Property("SomeIntProperty", 1).AddRule(_ => true, faultDetailC)
            .Build();

        Assert.Collection(faults,
            x => Assert.Equal(faultDetailA, x.Detail),
            x => Assert.Equal(faultDetailB1, x.Detail),
            x => Assert.Equal(faultDetailB2, x.Detail),
            x => Assert.Equal(faultDetailC, x.Detail));
    }
    
    [Fact]
    public void Build_NoFaults_ReturnsNoFaults()
    {
        const string faultDetailA = "faultA";
        const string faultDetailB1 = "faultB1";
        const string faultDetailB2 = "faultB2";
        const string faultDetailC = "faultC";
        
        IEnumerable<DomainRuleFault> faults = DomainRules.CreateBuilder()
            .Property<string>("SomeStringProperty", "test").AddRule(_ => false, faultDetailA)
            .Property<string?>("SomeOptionalStringProperty", null).AddRule(_ => false, faultDetailB1).AddRule(_ => false, faultDetailB2)
            .Property("SomeIntProperty", 1).AddRule(_ => false, faultDetailC)
            .Build();

        Assert.Empty(faults);
    }

    [Fact]
    public void Build_NoProperties_ReturnsNoFaults()
    {
        IEnumerable<DomainRuleFault> faults = DomainRules.CreateBuilder()
            .Build();

        Assert.Empty(faults);
    }

    [Fact]
    public void Build_NoRules_ReturnsNoFaults()
    {
        IEnumerable<DomainRuleFault> faults = DomainRules.CreateBuilder()
            .Property<string>("SomeStringProperty", "test")
            .Property<string?>("SomeOptionalStringProperty", null)
            .Property("SomeIntProperty", 1)
            .Build();

        Assert.Empty(faults);
    }
    
    [Fact]
    public void Build_SomeFaults_ReturnsAllFaults()
    {
        const string faultDetailA = "faultA";
        const string faultDetailB1 = "faultB1";
        const string faultDetailB2 = "faultB2";
        const string faultDetailC = "faultC";
        
        IEnumerable<DomainRuleFault> faults = DomainRules.CreateBuilder()
            .Property<string>("SomeStringProperty", "test").AddRule(_ => true, faultDetailA)
            .Property<string>("SomeOtherStringProperty", "test").AddRule(_ => false, faultDetailA)
            .Property<string?>("SomeOptionalStringProperty", null).AddRule(_ => true, faultDetailB1).AddRule(_ => true, faultDetailB2)
            .Property("SomeIntProperty", 1).AddRule(_ => true, faultDetailC)
            .Property("SomeOtherIntProperty", 1).AddRule(_ => false, faultDetailC)
            .Build();

        Assert.Collection(faults,
            x => Assert.Equal(faultDetailA, x.Detail),
            x => Assert.Equal(faultDetailB1, x.Detail),
            x => Assert.Equal(faultDetailB2, x.Detail),
            x => Assert.Equal(faultDetailC, x.Detail));
    }
}