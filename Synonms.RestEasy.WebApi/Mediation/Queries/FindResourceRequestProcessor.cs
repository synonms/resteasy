using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Persistence;
using MediatR;
using Synonms.Functional;
using Synonms.RestEasy.WebApi.Application;
using Synonms.RestEasy.WebApi.Schema.Resources;

namespace Synonms.RestEasy.WebApi.Mediation.Queries;

public class FindResourceRequestProcessor<TAggregateRoot, TResource> : IRequestHandler<FindResourceRequest<TAggregateRoot, TResource>, FindResourceResponse<TAggregateRoot, TResource>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    private readonly IAggregateRepository<TAggregateRoot> _aggregateRepository;
    private readonly IResourceMapper<TAggregateRoot, TResource> _resourceMapper;

    public FindResourceRequestProcessor(IAggregateRepository<TAggregateRoot> aggregateRepository, IResourceMapper<TAggregateRoot, TResource> resourceMapper)
    {
        _aggregateRepository = aggregateRepository;
        _resourceMapper = resourceMapper;
    }
    
    public async Task<FindResourceResponse<TAggregateRoot, TResource>> Handle(FindResourceRequest<TAggregateRoot, TResource> request, CancellationToken cancellationToken)
    {
        Maybe<TAggregateRoot> findOutcome = await _aggregateRepository.FindAsync(request.Id, cancellationToken);
            
        EntityTag entityTag = EntityTag.Uninitialised;
        
        Maybe<TResource> mapOutcome = findOutcome.Match(
            aggregateRoot =>
            {
                entityTag = aggregateRoot.EntityTag;
                return _resourceMapper.Map(aggregateRoot);
            },
            () => Maybe<TResource>.None);

        FindResourceResponse<TAggregateRoot, TResource> response = new(mapOutcome, entityTag);

        return response;
    }
}