using System.Collections.Concurrent;

namespace Synonms.RestEasy.Abstractions.Domain;

public interface IDomainEventProducer
{
    public IProducerConsumerCollection<DomainEvent> DomainEvents { get; }
}