using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Domain.ValueObjects;

namespace Synonms.RestEasy.Core.Tests.Unit.Shared;

public class TestAggregateRoot : AggregateRoot<TestAggregateRoot>
{
    public TestAggregateRoot()
    {
    }
        
    public TestAggregateRoot(EntityId<TestAggregateRoot> id)
    {
        Id = id;
    }
    
    public Moniker Name { get; set; } = Moniker.Convert(string.Empty);

    public void Update(string name)
    {
        Moniker moniker = Moniker.Convert(name);
        
        UpdateMandatoryValue(_ => _.Name, moniker);
    }
}