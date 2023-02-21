using MediatR;
using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;

namespace Synonms.RestEasy.Domain;

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
            // TODO
            return Maybe<Fault>.Some(new Fault("code", "title", "detail", new FaultSource()));
        }
        
        return await _mediator.Send(domainEventNotification);
    }
       
    private static IRequest<Maybe<Fault>>? CreateDomainEventRequest(DomainEvent domainEvent)
    {
        Type genericDispatcherType = typeof(DomainEventRequest<>).MakeGenericType(domainEvent.GetType());
        
        return (IRequest<Maybe<Fault>>?)Activator.CreateInstance(genericDispatcherType, domainEvent);
    }
}