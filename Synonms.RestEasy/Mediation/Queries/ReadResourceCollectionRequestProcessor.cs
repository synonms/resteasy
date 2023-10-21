using System.Linq.Expressions;
using System.Reflection;
using MediatR;
using Synonms.RestEasy.Abstractions.Application;
using Synonms.RestEasy.Abstractions.Domain;
using Synonms.RestEasy.Abstractions.Persistence;
using Synonms.RestEasy.Abstractions.Schema;
using Synonms.RestEasy.SharedKernel.Collections;

namespace Synonms.RestEasy.Mediation.Queries;

public class ReadResourceCollectionRequestProcessor<TAggregateRoot, TResource> : IRequestHandler<ReadResourceCollectionRequest<TAggregateRoot, TResource>, ReadResourceCollectionResponse<TAggregateRoot, TResource>>
    where TAggregateRoot : AggregateRoot<TAggregateRoot>
    where TResource : Resource
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
        PaginatedList<TAggregateRoot> paginatedAggregateRoots = (request.Parameters?.Any() ?? false)
            ? PaginatedList<TAggregateRoot>.Create(await _readRepository.QueryAsync(ParametersPredicate(request.Parameters)), request.Offset, request.Limit)
            : await _readRepository.ReadAsync(request.Offset, request.Limit);
        
        List<TResource> resources = paginatedAggregateRoots.Select(x => _resourceMapper.Map(request.HttpContext, x)).ToList();
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
            PropertyInfo? aggregatePropertyInfo = typeof(TAggregateRoot).GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (aggregatePropertyInfo is null)
            {
                continue;
            }

            // x.{columnName}
            MemberExpression columnNameExpression = Expression.Property(xParameter, aggregatePropertyInfo.Name);

            // {searchValue}
            ConstantExpression expectedValueExpression = Expression.Constant(expectedValue);

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
    
    private static bool ParametersPredicate(TAggregateRoot aggregateRoot, IReadOnlyDictionary<string, object>? parameters)
    {
        if ((parameters?.Any() ?? false) is false)
        {
            return true;
        }
        
        foreach ((string key, object expectedValue) in parameters)
        {
            PropertyInfo? aggregatePropertyInfo = typeof(TAggregateRoot).GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (aggregatePropertyInfo is null)
            {
                continue;
            }

            object? actualValue = aggregatePropertyInfo.GetValue(aggregateRoot);

            if (actualValue?.Equals(expectedValue) is false)
            {
                return false;
            }
        }

        return true;
    }
}
