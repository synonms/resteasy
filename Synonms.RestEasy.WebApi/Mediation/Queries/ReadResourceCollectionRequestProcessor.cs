using System.Linq.Expressions;
using System.Reflection;
using Synonms.RestEasy.Core.Collections;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Extensions;
using Synonms.RestEasy.Core.Persistence;
using Synonms.RestEasy.WebApi.Linq;
using MediatR;
using Synonms.RestEasy.Core.Application;
using Synonms.RestEasy.Core.Schema.Resources;
using Synonms.RestEasy.WebApi.Application;

namespace Synonms.RestEasy.WebApi.Mediation.Queries;

public class ReadResourceCollectionRequestProcessor<TAggregateRoot, TResource> : IRequestHandler<ReadResourceCollectionRequest<TAggregateRoot, TResource>, ReadResourceCollectionResponse<TAggregateRoot, TResource>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
{
    private readonly IAggregateRepository<TAggregateRoot> _aggregateRepository;
    private readonly IResourceMapper<TAggregateRoot, TResource> _resourceMapper;

    public ReadResourceCollectionRequestProcessor(IAggregateRepository<TAggregateRoot> aggregateRepository, IResourceMapper<TAggregateRoot, TResource> resourceMapper)
    {
        _aggregateRepository = aggregateRepository;
        _resourceMapper = resourceMapper;
    }

    public async Task<ReadResourceCollectionResponse<TAggregateRoot, TResource>> Handle(ReadResourceCollectionRequest<TAggregateRoot, TResource> request, CancellationToken cancellationToken)
    {
        IReadOnlyDictionary<string, object> filterParameters = request.QueryParameters.GetFiltersOnly();
        
        PaginatedList<TAggregateRoot> paginatedAggregateRoots = filterParameters.Any()
            ? PaginatedList<TAggregateRoot>.Create(_aggregateRepository.Query(ParametersPredicate(filterParameters)).ApplySort(request.SortItems), request.Offset, request.Limit)
            : await _aggregateRepository.ReadAllAsync(request.Offset, request.Limit, q => q.ApplySort(request.SortItems), cancellationToken);
        
        List<TResource> resources = paginatedAggregateRoots.Select(x => _resourceMapper.Map(x)).ToList();
        PaginatedList<TResource> paginatedResources = PaginatedList<TResource>.Create(resources, request.Offset, request.Limit, paginatedAggregateRoots.Size);

        ReadResourceCollectionResponse<TAggregateRoot, TResource> response = new(paginatedResources);

        return response;
    }
    
    private static Expression<Func<TAggregateRoot, bool>> ParametersPredicate(IReadOnlyDictionary<string, object>? parameters)
    {
        if ((parameters?.Any() ?? false) is false)
        {
            return _ => true;
        }

        // (TAggregateRoot x)
        ParameterExpression xParameter = Expression.Parameter(typeof(TAggregateRoot), "x");

        List<Expression> columnExpressions = new();

        foreach ((string key, object expectedValue) in parameters)
        {
            // TODO: Nested objects e.g. [child.property] = value

            PropertyInfo? aggregatePropertyInfo = typeof(TAggregateRoot).GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (aggregatePropertyInfo is null)
            {
                continue;
            }
            
            // x.{columnName}
            MemberExpression columnNameExpression = Expression.Property(xParameter, aggregatePropertyInfo.Name);

            if (aggregatePropertyInfo.PropertyType.IsValueObject())
            {
                // x.{columnName}.Value
                columnNameExpression = Expression.Property(columnNameExpression, "Value");
            }

            // {searchValue}
            ConstantExpression expectedValueExpression = Expression.Constant(expectedValue);
            
            // TODO: Anything other than string likely breaks
            // Need to convert the incoming value (likely a string) to the same type as the underlying ValueType value

            // x.{columnName} == {searchValue}
            BinaryExpression equalExpression = Expression.Equal(columnNameExpression, expectedValueExpression);

            columnExpressions.Add(equalExpression);
        }

        if (columnExpressions.Count == 1)
        {
            Expression<Func<TAggregateRoot, bool>> lambdaExpression = Expression.Lambda<Func<TAggregateRoot, bool>>(columnExpressions[0], new ParameterExpression[] { xParameter });

            return lambdaExpression;
        }
        else
        {
            Expression combinedExpression = columnExpressions[0];

            for (int i = 1; i < columnExpressions.Count; i++)
            {
                combinedExpression = Expression.And(combinedExpression, columnExpressions[i]);
            }

            Expression<Func<TAggregateRoot, bool>> lambdaExpression = Expression.Lambda<Func<TAggregateRoot, bool>>(combinedExpression, new ParameterExpression[] { xParameter });

            return lambdaExpression;
        }
    }
}