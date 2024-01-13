using Synonms.RestEasy.Core.Domain.Events;
using Synonms.RestEasy.Core.Domain.Faults;
using MediatR;
using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Domain.Events;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;
    
    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Maybe<Fault>> DispatchAsync(DomainEvent domainEvent)
    {
        IRequest<Maybe<Fault>>? domainEventNotification = CreateDomainEventRequest(domainEvent);

        if (domainEventNotification is null)
        {
            DomainEventFault fault = new("Unable to create request for domain event type {0}", domainEvent.GetType());
            return fault;
        }
        
        return await _mediator.Send(domainEventNotification);
    }
       
    private static IRequest<Maybe<Fault>>? CreateDomainEventRequest(DomainEvent domainEvent)
    {
        Type genericDispatcherType = typeof(DomainEventRequest<>).MakeGenericType(domainEvent.GetType());
        
        return (IRequest<Maybe<Fault>>?)Activator.CreateInstance(genericDispatcherType, domainEvent);
    }
}