using System.Linq.Expressions;
using System.Reflection;
using Synonms.RestEasy.Core.Domain;
using Synonms.RestEasy.Core.Extensions;
using Synonms.RestEasy.WebApi.Pipeline;

namespace Synonms.RestEasy.WebApi.Linq;

public static class QueryableExtensions
{
    public static IQueryable<TAggregateRoot> ApplySort<TAggregateRoot>(this IQueryable<TAggregateRoot> sourceQuery, IEnumerable<SortItem> sortItems)
        where TAggregateRoot : AggregateRoot<TAggregateRoot>
    {
        IQueryable<TAggregateRoot> orderedQuery = sourceQuery;
        
        foreach (SortItem sortItem in sortItems)
        {
            // (TAggregateRoot x)
            ParameterExpression xParameter = Expression.Parameter(typeof(TAggregateRoot), "x");
            
            PropertyInfo? aggregatePropertyInfo = typeof(TAggregateRoot).GetProperty(sortItem.PropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (aggregatePropertyInfo is null)
            {
                continue;
            }
            
            // x.{PropertyName}
            MemberExpression columnNameExpression = Expression.Property(xParameter, aggregatePropertyInfo.Name);
            
            if (aggregatePropertyInfo.PropertyType.IsValueObject())
            {
                // x.{PropertyName}.Value
                columnNameExpression = Expression.Property(columnNameExpression, "Value");
            }

            // x.{PropertyName}[.Value] as object
            UnaryExpression convertedColumnExpression = Expression.Convert(columnNameExpression, typeof(object));

            // (TAggregateRoot x) => x.{PropertyName}
            Expression<Func<TAggregateRoot, object>> lambdaExpression = Expression.Lambda<Func<TAggregateRoot, object>>(convertedColumnExpression, new ParameterExpression[] { xParameter });

            orderedQuery = sortItem.Direction == SortItem.SortDirection.Descending 
                ? orderedQuery.OrderByDescending(lambdaExpression) 
                : orderedQuery.OrderBy(lambdaExpression);
        }

        return orderedQuery;
    }
}