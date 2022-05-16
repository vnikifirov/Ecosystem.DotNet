using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Business.Services.Domain.Responses;

namespace Business.Extentions
{
    public static class IQueryableExtentions
    {
        public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, IQueryObject queryObj, Dictionary<string, Expression<Func<T, object>>> columnsMap)
        {
            if (queryObj is null || String.IsNullOrWhiteSpace(queryObj.SortBy) || !columnsMap.ContainsKey(queryObj.SortBy))
                return query;

            if (queryObj.IsSortAcsending)
                return query.OrderBy(columnsMap[queryObj.SortBy]);
            else
                return query.OrderByDescending(columnsMap[queryObj.SortBy]);
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int? skip = 0, int? take = 20)
        {
            if (!(skip.HasValue) || (skip < 0))
                skip = 0;

            if (!(take.HasValue) || (take <= 0))
                take = 10;

            return query.Skip(skip.Value).Take(take.Value);
        }

        // TODO: You need to use SOLID, because this method depends / tightly coupled to implementation of Project and ProjectQuery
        public static IQueryable<Context.Models.Project> ApplyFilteringByExactValue(this IQueryable<Context.Models.Project> query, ProjectQuery queryObj)
        {
            if (!(queryObj is null) && queryObj.ProjectId.HasValue)
                query = query.Where(p => p.Id == queryObj.ProjectId.Value);

            if (!(queryObj is null) && !String.IsNullOrWhiteSpace(queryObj.Name))
                query = query.Where(p => p.Name.ToLower() == queryObj.Name.ToLower());
          
            if (!(queryObj is null) && queryObj.Priority >= 1)
                query = query.Where(p => p.Priority == queryObj.Priority);

            if (!(queryObj is null) && !(queryObj.Status is null))
                query = query.Where(p => p.Status == queryObj.Status);

            return query;
        }
    }
}
