using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Domain.Faults;
using Synonms.RestEasy.Core.Persistence;
using MediatR;
using Synonms.Functional;
using Synonms.Functional.Extensions;
using Synonms.RestEasy.WebApi.Domain;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Mediation.Commands;

public class UpdateResourceRequestProcessor<TAggregateRoot, TResource> : IRequestHandler<UpdateResourceRequest<TAggregateRoot, TResource>, UpdateResourceResponse<TAggregateRoot>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    private readonly IAggregateRepository<TAggregateRoot> _aggregateRepository;
    private readonly IAggregateUpdater<TAggregateRoot, TResource> _aggregateUpdater;

    public UpdateResourceRequestProcessor(IAggregateRepository<TAggregateRoot> aggregateRepository, IAggregateUpdater<TAggregateRoot, TResource> aggregateUpdater)
    {
        _aggregateRepository = aggregateRepository;
        _aggregateUpdater = aggregateUpdater;
    }

    public async Task<UpdateResourceResponse<TAggregateRoot>> Handle(UpdateResourceRequest<TAggregateRoot, TResource> request, CancellationToken cancellationToken)
    {
        Result<TAggregateRoot> findOutcome = await _aggregateRepository.FindAsync(request.Id, cancellationToken)
            .MatchAsync(
                Result<TAggregateRoot>.SuccessAsync,
                () =>
                {
                    EntityNotFoundFault fault = new("{0} with id '{1}' not found.", nameof(TAggregateRoot), request.Id);
                    return Result<TAggregateRoot>.FailureAsync(fault);
                });

        Result<TAggregateRoot> editOutcome = await findOutcome
            .BindAsync(async aggregateRoot => (await _aggregateUpdater.UpdateAsync(aggregateRoot, request.Resource, cancellationToken)).ToResult(() => aggregateRoot));

        Result<TAggregateRoot> persistOutcome = await editOutcome
            .BindAsync(async aggregateRoot =>
            {
                await _aggregateRepository.UpdateAsync(aggregateRoot, cancellationToken);
                await _aggregateRepository.SaveChangesAsync(cancellationToken);
                
                return Result<TAggregateRoot>.Success(aggregateRoot);
            });

        UpdateResourceResponse<TAggregateRoot> response = new(persistOutcome);

        return response;
    }
}