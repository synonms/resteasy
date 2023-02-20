using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Infrastructure;

namespace Synonms.RestEasy.Extensions;

public static class AggregateMemberExtensions
{
    public static Maybe<Fault> Merge<TAggregateMember, TChildResource>(this ICollection<TAggregateMember> aggregateMemberCollection, 
        IEnumerable<TChildResource> childResources, 
        Func<TAggregateMember, TChildResource, bool> matchFunc,
        Func<TChildResource, Result<TAggregateMember>> createFunc,
        Func<TAggregateMember, TChildResource, Maybe<Fault>> editFunc)
        where TAggregateMember : AggregateMember<TAggregateMember>
        where TChildResource : ChildResource<TAggregateMember>
    {
        List<TChildResource> materialisedResources = childResources.ToList();
        
        List<TAggregateMember> membersToDelete = aggregateMemberCollection
            .Where(aggregateMember => materialisedResources.All(dto => matchFunc(aggregateMember, dto) is false))
            .ToList();

        foreach (TAggregateMember aggregateMember in membersToDelete)
        {
            if (aggregateMemberCollection.Remove(aggregateMember) is false)
            {
                return new InternalFault($"Failed to remove {nameof(TAggregateMember)} id [{aggregateMember.Id.Value}].");
            }
        }

        foreach (TAggregateMember aggregateMember in aggregateMemberCollection)
        {
            TChildResource? resource = materialisedResources.SingleOrDefault(dto => matchFunc(aggregateMember, dto));

            if (resource is not null)
            {
                Fault? editFault = editFunc(aggregateMember, resource).Match(fault => fault, () => null as Fault);

                if (editFault is not null)
                {
                    return editFault;
                }
            }
        }

        List<TChildResource> resourcesToAdd = materialisedResources
            .Where(dto => aggregateMemberCollection.All(aggregateMember => matchFunc(aggregateMember, dto) is false))
            .ToList();

        return resourcesToAdd
            .Select(createFunc)
            .Reduce(aggregateMembersToAdd => aggregateMembersToAdd)
            .Match(
                aggregateMembersToAdd =>
                {
                    foreach (TAggregateMember aggregateMember in aggregateMembersToAdd)
                    {
                        aggregateMemberCollection.Add(aggregateMember);
                    }

                    return Maybe<Fault>.None;
                },
                fault => fault);
    }
}