using MediatR;
using Synonms.Functional;
using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.Abstractions.Schema.Server;

namespace Synonms.RestEasy.Mediation.Queries;

public class FindResourceRequestProcessor<TAggregateRoot, TResource> : IRequestHandler<FindResourceRequest<TAggregateRoot, TResource>, FindResourceResponse<TAggregateRoot, TResource>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : ServerResource<TAggregateRoot>
{
    private readonly IReadRepository<TAggregateRoot> _readRepository;
    private readonly IResourceMapper<TAggregateRoot, TResource> _resourceMapper;

    public FindResourceRequestProcessor(IReadRepository<TAggregateRoot> readRepository, IResourceMapper<TAggregateRoot, TResource> resourceMapper)
    {
        _readRepository = readRepository;
        _resourceMapper = resourceMapper;
    }
    
    public async Task<FindResourceResponse<TAggregateRoot, TResource>> Handle(FindResourceRequest<TAggregateRoot, TResource> request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Handling FindResourceRequest for AggregateRoot [{0}]...", typeof(TAggregateRoot).Name);
        
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

        Maybe<TResource> mapOutcome = filterOutcome.Match(
            aggregateRoot => _resourceMapper.Map(request.HttpContext, aggregateRoot),
            () => Maybe<TResource>.None);

        FindResourceResponse<TAggregateRoot, TResource> response = new(mapOutcome);

        return response;
    }
}