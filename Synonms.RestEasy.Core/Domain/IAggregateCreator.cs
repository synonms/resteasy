using Synonms.Functional;
using Synonms.RestEasy.Core.Schema.Resources;

namespace Synonms.RestEasy.Core.Domain;

public interface IAggregateCreator<TAggregateRoot, in TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    Task<Result<TAggregateRoot>> CreateAsync(TResource resource, CancellationToken cancellationToken);
}