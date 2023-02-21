using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Abstractions.Domain;

public interface IAggregateCreator<TAggregateRoot, in TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    Task<Result<TAggregateRoot>> CreateAsync(TResource resource, CancellationToken cancellationToken);
}