using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace System.Linq;

public static class LinqExtensions
{

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool predicate, Expression<Func<T, bool>> where)
    {
        if (predicate)
            return query.Where(where);
        return query;
    }

    public static IQueryable<T> OrderByIf<T>(this IQueryable<T> query, bool predicate, Expression<Func<T, object>> where)
    {
        if (predicate)
            return query.OrderBy(where);
        return query;
    }

    public static IQueryable<T> OrderByDescendingIf<T>(this IQueryable<T> query, bool predicate, Expression<Func<T, object>> where)
    {
        if (predicate)
            return query.OrderByDescending(where);
        return query;
    }

    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> query, bool predicate, Func<T, bool> where)
    {
        if (predicate)
            return query.Where(where);
        return query;
    }
}