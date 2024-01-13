using Synonms.RestEasy.Core.Domain.Events;
using MediatR;
using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Domain.Events;

public class DomainEventRequest<TDomainEvent> : IRequest<Maybe<Fault>> 
    where TDomainEvent : DomainEvent
{
    public DomainEventRequest(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
    
    public TDomainEvent DomainEvent { get; }
}