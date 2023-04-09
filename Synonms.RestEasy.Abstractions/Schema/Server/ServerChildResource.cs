using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Abstractions.Schema.Server;

public abstract class ServerChildResource<TAggregateMember>
    where TAggregateMember : AggregateMember<TAggregateMember>
{
    protected ServerChildResource()
    {
        Id = EntityId<TAggregateMember>.Uninitialised;
    }
    
    protected ServerChildResource(EntityId<TAggregateMember> id)
    {
        Id = id;
    }

    public EntityId<TAggregateMember> Id { get; init; }
}