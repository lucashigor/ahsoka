namespace Ahsoka.Infrastructure.Repositories.Common;
using Ahsoka.Domain.SeedWork;
using Ahsoka.Domain.SeedWork.Repository.ISearchableRepository;
using Ahsoka.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class QueryHelper<TEntity, TEntityId>(PrincipalContext context) 
    where TEntity : Entity<TEntityId> 
    where TEntityId : IEquatable<TEntityId>
{
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    protected virtual IQueryable<TEntity> GetMany(Expression<Func<TEntity, bool>> where)
        => _dbSet.AsNoTracking().Where(where);

    protected virtual IQueryable<TEntity> GetManyPaginated(Expression<Func<TEntity, bool>> where,
        string? orderBy,
        SearchOrder order,
        int page,
        int perPage,
        out int totalPages)
        => GetManyPaginated(where, orderBy, order, page, perPage, null!, out totalPages);

    protected virtual IQueryable<TEntity> GetManyPaginated(Expression<Func<TEntity, bool>> where,
        string? orderBy,
        SearchOrder order,
        int page,
        int perPage,
        Expression<Func<TEntity, object>> include,
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
            var field = typeof(TEntity).GetProperties()
                .ToList()
                .FirstOrDefault(x => x.Name.ToLower()
                .Equals(orderBy.ToLower()));

            if (field != null)
            {
                if (order == SearchOrder.Asc)
                    query = query.OrderBy(ToLambda<TEntity>(field.Name));

                if (order == SearchOrder.Desc)
                    query = query.OrderByDescending(ToLambda<TEntity>(field.Name));
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

    private static Expression<Func<TProp, object>> ToLambda<TProp>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var property = Expression.Property(parameter, propertyName);
        var propAsObject = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<TProp, object>>(propAsObject, parameter);
    }
}

