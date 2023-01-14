using MediatR;
using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.Abstractions.Schema;

namespace Synonms.RestEasy.Mediation.Commands;

public abstract class CreateResourceRequestProcessor<TAggregateRoot, TResource> : IRequestHandler<CreateResourceRequest<TAggregateRoot, TResource>, CreateResourceResponse<TAggregateRoot>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    private readonly ICreateRepository<TAggregateRoot> _createRepository;
    private readonly IAggregateCreator<TAggregateRoot, TResource> _aggregateCreator;

    public CreateResourceRequestProcessor(ICreateRepository<TAggregateRoot> createRepository, IAggregateCreator<TAggregateRoot, TResource> aggregateCreator)
    {
        _createRepository = createRepository;
        _aggregateCreator = aggregateCreator;
    }

    public async Task<CreateResourceResponse<TAggregateRoot>> Handle(CreateResourceRequest<TAggregateRoot, TResource> request, CancellationToken cancellationToken)
    {
        // TODO: Validation
        Result<TAggregateRoot> createOutcome = _aggregateCreator.Create(request.Resource);
        
        Result<TAggregateRoot> persistOutcome = await createOutcome
            .MatchAsync(
                async aggregateRoot =>
                {
                    EntityId<TAggregateRoot> entityId = await _createRepository.CreateAsync(aggregateRoot);
                    return Result<TAggregateRoot>.Success(aggregateRoot);
                },
                Result<TAggregateRoot>.FailureAsync);
        
        CreateResourceResponse<TAggregateRoot> response = new(persistOutcome);

        return response;
    }
}