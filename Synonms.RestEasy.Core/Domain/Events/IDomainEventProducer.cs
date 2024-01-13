using System.Collections.Concurrent;

namespace Synonms.RestEasy.Core.Domain.Events;

public interface IDomainEventProducer
{
    public IProducerConsumerCollection<DomainEvent> DomainEvents { get; }
}