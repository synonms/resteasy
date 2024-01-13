using Synonms.RestEasy.Core.Domain;

namespace Synonms.RestEasy.Core.Tests.Unit.Shared;

public class TestAggregateMember : AggregateMember<TestAggregateMember>
{
    public TestAggregateMember() : this(EntityId<TestAggregateMember>.New())
    {
    }

    public TestAggregateMember(EntityId<TestAggregateMember> id)
    {
        Id = id;
    }
}