namespace Ahsoka.Infrastructure;
using System.Linq.Expressions;
using Ahsoka.Domain;
using Microsoft.EntityFrameworkCore;

public class QueryHelper<T, R> where T : Entity<R> where R : IEquatable<R>
{
    protected readonly DbSet<T> _dbSet;

    public QueryHelper(PrincipalContext context)
    {
        _dbSet = context.Set<T>();
    }

    protected virtual IQueryable<T> GetMany(Expression<Func<T, bool>> where)
        => _dbSet.AsNoTracking().Where(where);
        
    protected virtual IQueryable<T> GetManyPaginated(Expression<Func<T, bool>> where,
        string? orderBy,
        SearchOrder order,
        int page,
        int perPage,
        out int totalPages)
        => GetManyPaginated(where, orderBy, order, page, perPage, null!, out totalPages);

    protected virtual IQueryable<T> GetManyPaginated(Expression<Func<T, bool>> where,
        string? orderBy,
        SearchOrder order,
        int page,
        int perPage,
        Expression<Func<T, object>> include,
        out int totalPages)
    {
        var query = _dbSet.AsNoTracking();

        if (where != null)
        {
            query = query.Where(where);
        }

        totalPages = query.Count();

        if (string.IsNullOrEmpty(orderBy) is false)
        {
            var field = typeof(T).GetProperties()
                .ToList()
                .FirstOrDefault(x => x.Name.ToLower()
                .Equals(orderBy.ToLower()));

            if (field != null)
            {
                if (order == SearchOrder.Asc)
                    query = query.OrderBy(ToLambda<T>(field.Name));

                if (order == SearchOrder.Desc)
                    query = query.OrderByDescending(ToLambda<T>(field.Name));
            }
        }
        else
        {
            query = query.OrderBy(z => z.Id);
        }

        if (include != null)
        {
            query = query.Include(include);
        }

        return query.Skip(page * perPage).Take(perPage);
    }

    private static Expression<Func<R, object>> ToLambda<R>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(T));
        var property = Expression.Property(parameter, propertyName);
        var propAsObject = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<R, object>>(propAsObject, parameter);
    }
}

