using MediatR;
using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.SharedKernel.Collections;

namespace Synonms.RestEasy.Mediation.Queries;

public abstract class ReadChildResourceCollectionRequestProcessor<TParentEntity, TAggregateRoot, TResource> : IRequestHandler<ReadChildResourceCollectionRequest<TParentEntity, TAggregateRoot, TResource>, ReadChildResourceCollectionResponse<TParentEntity, TAggregateRoot, TResource>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TParentEntity : Entity<TParentEntity>
    where TResource : Resource<TAggregateRoot>
{
    private readonly IReadRepository<TAggregateRoot> _readRepository;
    private readonly IResourceMapper<TAggregateRoot, TResource> _resourceMapper;

    public ReadChildResourceCollectionRequestProcessor(IReadRepository<TAggregateRoot> readRepository, IResourceMapper<TAggregateRoot, TResource> resourceMapper)
    {
        _readRepository = readRepository;
        _resourceMapper = resourceMapper;
    }

    public async Task<ReadChildResourceCollectionResponse<TParentEntity, TAggregateRoot, TResource>> Handle(ReadChildResourceCollectionRequest<TParentEntity, TAggregateRoot, TResource> request, CancellationToken cancellationToken)
    {
        // TODO: Filter by ParentId
        
        PaginatedList<TAggregateRoot> paginatedAggregateRoots = await _readRepository.ReadAsync(request.Offset, request.Limit);
        List<TResource> resources = paginatedAggregateRoots.Select(x => _resourceMapper.Map(request.HttpContext, x)).ToList();
        PaginatedList<TResource> paginatedResources = PaginatedList<TResource>.Create(resources, request.Offset, request.Limit, resources.Count);

        ReadChildResourceCollectionResponse<TParentEntity, TAggregateRoot, TResource> response = new(paginatedResources);

        return response;
    }
}