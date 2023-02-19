using MediatR;
using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.SharedKernel.Collections;

namespace Synonms.RestEasy.Mediation.Queries;

public class ReadResourceCollectionRequestProcessor<TAggregateRoot, TResource> : IRequestHandler<ReadResourceCollectionRequest<TAggregateRoot, TResource>, ReadResourceCollectionResponse<TAggregateRoot, TResource>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource<TAggregateRoot>
{
    private readonly IReadRepository<TAggregateRoot> _readRepository;
    private readonly IResourceMapper<TAggregateRoot, TResource> _resourceMapper;

    public ReadResourceCollectionRequestProcessor(IReadRepository<TAggregateRoot> readRepository, IResourceMapper<TAggregateRoot, TResource> resourceMapper)
    {
        _readRepository = readRepository;
        _resourceMapper = resourceMapper;
    }

    public async Task<ReadResourceCollectionResponse<TAggregateRoot, TResource>> Handle(ReadResourceCollectionRequest<TAggregateRoot, TResource> request, CancellationToken cancellationToken)
    {
        PaginatedList<TAggregateRoot> paginatedAggregateRoots = await _readRepository.ReadAsync(request.Offset, request.Limit);
        
        List<TResource> resources = paginatedAggregateRoots.Select(x => _resourceMapper.Map(request.HttpContext, x)).ToList();
        PaginatedList<TResource> paginatedResources = PaginatedList<TResource>.Create(resources, request.Offset, request.Limit, paginatedAggregateRoots.Size);

        ReadResourceCollectionResponse<TAggregateRoot, TResource> response = new(paginatedResources);

        return response;
    }
}
