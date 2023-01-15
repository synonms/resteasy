using MediatR;
using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Mediation.Commands;

public class DeleteResourceRequestProcessor<TAggregateRoot> : IRequestHandler<DeleteResourceRequest<TAggregateRoot>, DeleteResourceResponse>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
{
    private readonly IReadRepository<TAggregateRoot> _readRepository;
    private readonly IDeleteRepository<TAggregateRoot> _deleteRepository;

    public DeleteResourceRequestProcessor(IReadRepository<TAggregateRoot> readRepository, IDeleteRepository<TAggregateRoot> deleteRepository)
    {
        _readRepository = readRepository;
        _deleteRepository = deleteRepository;
    }
    
    public async Task<DeleteResourceResponse> Handle(DeleteResourceRequest<TAggregateRoot> request, CancellationToken cancellationToken)
    {
        Maybe<TAggregateRoot> findOutcome = await _readRepository.FindAsync(request.Id);

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
                    await _deleteRepository.DeleteAsync(aggregateRoot.Id);
                    
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