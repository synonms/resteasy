namespace Synonms.RestEasy.Abstractions.Domain;

public abstract class AggregateMember<TAggregateMember> : Entity<TAggregateMember>
    where TAggregateMember : AggregateMember<TAggregateMember>
{
}
