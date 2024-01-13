using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Persistence;
using MediatR;
using Synonms.Functional;

namespace Synonms.RestEasy.WebApi.Mediation.Commands;

public class DeleteResourceRequestProcessor<TAggregateRoot> : IRequestHandler<DeleteResourceRequest<TAggregateRoot>, DeleteResourceResponse>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    private readonly IAggregateRepository<TAggregateRoot> _aggregateRepository;

    public DeleteResourceRequestProcessor(IAggregateRepository<TAggregateRoot> aggregateRepository)
    {
        _aggregateRepository = aggregateRepository;
    }
    
    public async Task<DeleteResourceResponse> Handle(DeleteResourceRequest<TAggregateRoot> request, CancellationToken cancellationToken)
    {
        Maybe<TAggregateRoot> findOutcome = await _aggregateRepository.FindAsync(request.Id, cancellationToken);

        Maybe<TAggregateRoot> filterOutcome = findOutcome.Bind(aggregateRoot =>
        {
            if (request.Filter is null)
            {
                return aggregateRoot;
            }

            if (request.Filter(aggregateRoot))
            {
                return aggregateRoot;
            }
            
            return Maybe<TAggregateRoot>.None;
        });

        Maybe<Fault> deleteOutcome = await filterOutcome
            .MatchAsync(
                async aggregateRoot => 
                {
                    await _aggregateRepository.DeleteAsync(aggregateRoot.Id, cancellationToken);
                    await _aggregateRepository.SaveChangesAsync(cancellationToken);

                    return Maybe<Fault>.None;
                },
                () =>
                {
                    EntityNotFoundFault fault = new ("{0} with id '{1}' not found.", nameof(TAggregateRoot), request.Id);

                    return Maybe<Fault>.SomeAsync(fault);
                });

        DeleteResourceResponse response = new (deleteOutcome);
        
        return response;
    }
}