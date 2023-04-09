using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Server;

namespace Synonms.RestEasy.Abstractions.Domain;

public interface IAggregateCreator<TAggregateRoot, in TResource>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : ServerResource<TAggregateRoot>
{
    Task<Result<TAggregateRoot>> CreateAsync(TResource resource, CancellationToken cancellationToken);
}