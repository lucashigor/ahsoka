namespace Ahsoka.Domain.SeedWork.Repository;

public interface IRepository<T, R> where T : Entity<R> where R : IEquatable<R>
{
    Task InsertAsync(T entity, CancellationToken cancellationToken);
    Task UpdateAsync(T entity, CancellationToken cancellationToken);

    Task DeleteAsync(T entity, CancellationToken cancellationToken);
    Task DeleteAsync(R id, CancellationToken cancellationToken);

    Task<T?> GetByIdAsync(R id, CancellationToken cancellationToken);
}
