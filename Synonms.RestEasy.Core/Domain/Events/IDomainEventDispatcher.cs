using Synonms.Functional;

namespace Synonms.RestEasy.Core.Domain.Events;

public interface IDomainEventDispatcher
{
    Task<Maybe<Fault>> DispatchAsync(DomainEvent domainEvent);
}