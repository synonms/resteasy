using MediatR;
using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Server;
using Synonms.RestEasy.Domain.Faults;

namespace Synonms.RestEasy.Mediation.Commands;

public class UpdateResourceRequestProcessor<TAggregateRoot, TResource> : IRequestHandler<UpdateResourceRequest<TAggregateRoot, TResource>, UpdateResourceResponse<TAggregateRoot>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : ServerResource<TAggregateRoot>
{
    private readonly IReadRepository<TAggregateRoot> _readRepository;
    private readonly IUpdateRepository<TAggregateRoot> _updateRepository;
    private readonly IAggregateUpdater<TAggregateRoot, TResource> _aggregateUpdater;

    public UpdateResourceRequestProcessor(IReadRepository<TAggregateRoot> readRepository, IUpdateRepository<TAggregateRoot> updateRepository, IAggregateUpdater<TAggregateRoot, TResource> aggregateUpdater)
    {
        _readRepository = readRepository;
        _updateRepository = updateRepository;
        _aggregateUpdater = aggregateUpdater;
    }

    public async Task<UpdateResourceResponse<TAggregateRoot>> Handle(UpdateResourceRequest<TAggregateRoot, TResource> request, CancellationToken cancellationToken)
    {
        // TODO: Validation

        Result<TAggregateRoot> findOutcome = await _readRepository.FindAsync(request.Id)
            .MatchAsync(
                Result<TAggregateRoot>.SuccessAsync,
                () =>
                {
                    EntityNotFoundFault fault = new("{0} with id '{1}' not found.", nameof(TAggregateRoot), request.Id);
                    return Result<TAggregateRoot>.FailureAsync(fault);
                });

        Result<TAggregateRoot> filterOutcome = findOutcome
            .Bind(aggregateRoot =>
            {
                if (request.Filter is null)
                {
                    return aggregateRoot;
                }

                if (request.Filter(aggregateRoot))
                {
                    return aggregateRoot;
                }
            
                EntityNotFoundFault fault = new("{0} with id '{1}' does not match filter.", nameof(TAggregateRoot), request.Id);
                return Result<TAggregateRoot>.Failure(fault);
            });
            
        Result<TAggregateRoot> editOutcome = await filterOutcome
            .BindAsync(async aggregateRoot => (await _aggregateUpdater.UpdateAsync(aggregateRoot, request.Resource, cancellationToken)).ToResult(() => aggregateRoot));

        Result<TAggregateRoot> persistOutcome = await editOutcome
            .BindAsync(async aggregateRoot =>
            {
                await _updateRepository.UpdateAsync(aggregateRoot);
                return Result<TAggregateRoot>.Success(aggregateRoot);
            });

        UpdateResourceResponse<TAggregateRoot> response = new(persistOutcome);

        return response;
    }
}
