using System;
using System.Linq;
using System.Linq.Expressions;

namespace TourismSmartTransportation.Business.Extensions
{
    public static class Extensions
    {
        // OrderBy custom
        public static IQueryable<TEntity> OrderByDynamicProperty<TEntity>(this IQueryable<TEntity> source, string orderByProperty,
                          bool desc = false)
        {
            string command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "property");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}