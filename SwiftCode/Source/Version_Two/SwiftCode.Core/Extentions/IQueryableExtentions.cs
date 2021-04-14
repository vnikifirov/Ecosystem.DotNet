namespace SwiftCode.Core.Extentions
{
    using System.Linq;
    using SwiftCode.Core.Interfaces.Models.Request;

    public static class IQueryableExtentions
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> queryable, IQueryObject queryObj)
        {
            if (queryObj == null) return queryable;

            if (queryObj.PageNumber <= 0)
                queryObj.PageNumber = 1;

            if (queryObj.PageSize <= 0)
                queryObj.PageSize = 10;

            return queryable.Skip((queryObj.PageNumber - 1) * queryObj.PageSize).Take(queryObj.PageSize);
        }
    }
}
