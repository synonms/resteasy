namespace Synonms.RestEasy.Core.Domain;

public abstract class AggregateRoot<TAggregateRoot> : Entity<TAggregateRoot>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    public EntityTag EntityTag { get; private set; } = EntityTag.New();

    protected override void MarkAsUpdated()
    {
        base.MarkAsUpdated();
        EntityTag = EntityTag.New();
    }
}
