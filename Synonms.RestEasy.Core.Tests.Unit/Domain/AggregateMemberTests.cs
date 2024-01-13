using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Tests.Unit.Shared;
using Xunit;

namespace Synonms.RestEasy.Core.Tests.Unit.Domain;

public class AggregateMemberTests
{
    [Fact]
    public void Construction_WithId_SetsId()
    {
        EntityId<TestAggregateMember> id = EntityId<TestAggregateMember>.New();
        TestAggregateMember aggregateMember = new(id);
        
        Assert.Equal(id, aggregateMember.Id);
    }

    [Fact]
    public void Construction_WithoutId_NewIdIsGenerated()
    {
        TestAggregateMember aggregateMember = new();
        
        Assert.False(aggregateMember.Id.IsEmpty);
        Assert.NotEqual(Guid.Empty, aggregateMember.Id.Value);
    }
}