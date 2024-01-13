using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Persistence;
using MediatR;
using Synonms.Functional;
using Synonms.RestEasy.WebApi.Domain;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Mediation.Commands;

public class CreateResourceRequestProcessor<TAggregateRoot, TResource> : IRequestHandler<CreateResourceRequest<TAggregateRoot, TResource>, CreateResourceResponse<TAggregateRoot>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    private readonly IAggregateRepository<TAggregateRoot> _aggregateRepository;
    private readonly IAggregateCreator<TAggregateRoot, TResource> _aggregateCreator;

    public CreateResourceRequestProcessor(IAggregateRepository<TAggregateRoot> aggregateRepository, IAggregateCreator<TAggregateRoot, TResource> aggregateCreator)
    {
        _aggregateRepository = aggregateRepository;
        _aggregateCreator = aggregateCreator;
    }

    public async Task<CreateResourceResponse<TAggregateRoot>> Handle(CreateResourceRequest<TAggregateRoot, TResource> request, CancellationToken cancellationToken)
    {
        Result<TAggregateRoot> createOutcome = await _aggregateCreator.CreateAsync(request.Resource, cancellationToken);
        
        Result<TAggregateRoot> persistOutcome = await createOutcome
            .MatchAsync(
                async aggregateRoot =>
                {
                    await _aggregateRepository.AddAsync(aggregateRoot, cancellationToken);
                    await _aggregateRepository.SaveChangesAsync(cancellationToken);
                    
                    return Result<TAggregateRoot>.Success(aggregateRoot);
                },
                Result<TAggregateRoot>.FailureAsync);
        
        CreateResourceResponse<TAggregateRoot> response = new(persistOutcome);

        return response;
    }
}