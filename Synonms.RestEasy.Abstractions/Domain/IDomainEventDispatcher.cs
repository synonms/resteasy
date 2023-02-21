using Synonms.Functional;

namespace Synonms.RestEasy.Abstractions.Domain;

public interface IDomainEventDispatcher
{
    Task<Maybe<Fault>> DispatchAsync(DomainEvent domainEvent);
}