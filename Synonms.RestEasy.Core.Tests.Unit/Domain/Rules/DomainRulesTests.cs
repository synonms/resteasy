using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Domain.Rules;
using Synonms.Functional;
using Xunit;

namespace Synonms.RestEasy.Core.Tests.Unit.Domain.Rules;

public class DomainRulesTests
{
    private class SuccessfulRuleset : IDomainRuleset
    {
        public IEnumerable<DomainRuleFault> Apply() => Enumerable.Empty<DomainRuleFault>();
    }

    private class FailingRuleset : IDomainRuleset
    {
        private readonly int _noOfFaults;
        private readonly string _faultDetail;

        public FailingRuleset(int noOfFaults, string faultDetail)
        {
            _noOfFaults = noOfFaults;
            _faultDetail = faultDetail;
        }
        
        public IEnumerable<DomainRuleFault> Apply() => Enumerable.Repeat(new DomainRuleFault(_faultDetail), _noOfFaults);
    }

    [Fact]
    public void Apply_NoRules_ReturnsNone()
    {
        Maybe<Fault> maybeFault = DomainRules.Apply();
        
        Assert.True(maybeFault.IsNone);
    }

    [Fact]
    public void Apply_AllRulesPass_ReturnsNone()
    {
        Maybe<Fault> maybeFault = DomainRules
            .Apply
            (
                new SuccessfulRuleset(),
                new SuccessfulRuleset(),
                new SuccessfulRuleset()
            );
        
        Assert.True(maybeFault.IsNone);
    }
    
    [Fact]
    public void Apply_AllRulesFail_ReturnsAllFaults()
    {
        const string faultDetailA = "faultA";
        const string faultDetailB = "faultB";
        const string faultDetailC = "faultC";
        
        Maybe<Fault> maybeFault = DomainRules
            .Apply
            (
                new FailingRuleset(1, faultDetailA),
                new FailingRuleset(2, faultDetailB),
                new FailingRuleset(1, faultDetailC)
            );

        maybeFault.Match(
            fault =>
            {
                DomainRulesFault domainRulesFault = Assert.IsType<DomainRulesFault>(fault);
                Assert.Collection(domainRulesFault.Faults,
                    x => Assert.Equal(faultDetailA, x.Detail),
                    x => Assert.Equal(faultDetailB, x.Detail),
                    x => Assert.Equal(faultDetailB, x.Detail),
                    x => Assert.Equal(faultDetailC, x.Detail));
            },
            () => Assert.Fail("Expected Some"));
    }
    
    [Fact]
    public void Apply_SomeRulesFail_ReturnsAllFaults()
    {
        const string faultDetailA = "faultA";
        const string faultDetailB = "faultB";
        
        Maybe<Fault> maybeFault = DomainRules
            .Apply
            (
                new SuccessfulRuleset(),
                new FailingRuleset(1, faultDetailA),
                new FailingRuleset(2, faultDetailB),
                new SuccessfulRuleset()
            );

        maybeFault.Match(
            fault =>
            {
                DomainRulesFault domainRulesFault = Assert.IsType<DomainRulesFault>(fault);
                Assert.Collection(domainRulesFault.Faults,
                    x => Assert.Equal(faultDetailA, x.Detail),
                    x => Assert.Equal(faultDetailB, x.Detail),
                    x => Assert.Equal(faultDetailB, x.Detail));
            },
            () => Assert.Fail("Expected Some"));
    }
}