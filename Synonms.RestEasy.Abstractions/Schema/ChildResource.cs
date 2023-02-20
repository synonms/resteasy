using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Schema;

public abstract class ChildResource<TAggregateMember>
    where TAggregateMember : AggregateMember<TAggregateMember>
{
    protected ChildResource()
    {
        Id = EntityId<TAggregateMember>.Uninitialised;
    }
    
    protected ChildResource(EntityId<TAggregateMember> id)
    {
        Id = id;
    }

    public EntityId<TAggregateMember> Id { get; init; }
}