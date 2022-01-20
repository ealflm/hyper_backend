using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TourismSmartTransportation.Business.Extensions
{
    public static class Extensions
    {
        // OrderBy custom single field
        public static IQueryable<TEntity> OrderBySingleField<TEntity>(this IQueryable<TEntity> source, string orderByProperty,
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

        // OrderBy custom multiple fields
        public static IEnumerable<T> OrderByMultipleFields<T>(this IEnumerable<T> data, string orderQueryByString)
        {
            string[] orderParams = Regex.Split(orderQueryByString, @"\s*,\s*");
            var sortExpressions = new List<Tuple<string, string>>();

            foreach (var param in orderParams)
            {
                string[] fields = param.Split(' ');
                string field = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fields[0].Trim().ToLower());
                string status = fields.Length > 1 ? fields[1].Trim().ToLower() : "asc";
                sortExpressions.Add(new Tuple<string, string>(field, status));
            }

            if ((sortExpressions == null) || (sortExpressions.Count <= 0))
                return data;

            IEnumerable<T> query = from item in data select item;
            IOrderedEnumerable<T> orderedQuery = null;
            for (int i = 0; i < sortExpressions.Count; i++)
            {
                var index = i;
                Func<T, object> expression = item => item.GetType()
                 .GetProperty(sortExpressions[index].Item1)
                 .GetValue(item, null);
                if (sortExpressions[index].Item2 == "asc")
                {
                    orderedQuery = (index == 0) ? query.OrderBy(expression) :
                        orderedQuery.ThenBy(expression);
                }
                else
                {
                    orderedQuery = (index == 0) ? query.OrderByDescending(expression) :
                        orderedQuery.ThenByDescending(expression);
                }
            }
            query = orderedQuery;
            return query;
        }

        // OrderBy custom multiple fields
        public static IQueryable<T> OrderByCustomFunc<T>(this IQueryable<T> source, string sortBy) where T : class
        {
            return string.IsNullOrWhiteSpace(sortBy) ? source : source.OrderByMultipleFields(sortBy).AsQueryable();
        }

        // Paginate
        public static IQueryable<T> PaginateFunc<T>(this IQueryable<T> source, int pageIndex, int itemsPerPage)
        {
            var totalItems = source.Count();
            IQueryable<T> pagingQuery = source;
            if (totalItems > 0)
            {
                pagingQuery = source.Skip(itemsPerPage < totalItems ? itemsPerPage * Math.Max(pageIndex - 1, 0) : 0)
                                    .Take(itemsPerPage < totalItems && itemsPerPage > 0 ? itemsPerPage : totalItems)
                                    .AsQueryable();
            }

            return pagingQuery;
        }

        //
        public static IQueryable<T> FilterFunc<T>(this IQueryable<T> source, Object model)
        {
            if (model is null)
                return source;

            IQueryable<T> query = source;
            foreach (var p in model.GetType().GetProperties())
            {
                if (p.Name != "Title" && p.GetValue(model) == null)
                    continue;

                string[] spiltArr = Regex.Split(p.ToString(), @"\s*,\s*");
                for (int i = 0; i < spiltArr.Length; i++)
                {
                    var type = typeof(T);
                    var property = type.GetProperty(p.Name);
                    Expression<Func<T, bool>> predicate = item
                        => item.GetType().GetProperty(p.Name).GetValue(item, null).ToString().Contains(spiltArr[i]);

                    var resultExpression = Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { type }),
                                          source.Expression, Expression.Quote(predicate));

                    query = source.Provider.CreateQuery<T>(resultExpression);
                }
            }



            // Expression<Func<T, bool>> predicate = item => true;
            // var resultExpression = Expression.Call(
            //     null,
            //     ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(T) }),
            //     new Expression[] { source.Expression, Expression.Quote(predicate) });

            return query;
        }

    }
}