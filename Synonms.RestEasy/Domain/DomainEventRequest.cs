using MediatR;
using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Domain;

public class DomainEventRequest<TDomainEvent> : IRequest<Maybe<Fault>> 
    where TDomainEvent : DomainEvent
{
    public DomainEventRequest(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
    
    public TDomainEvent DomainEvent { get; }
}