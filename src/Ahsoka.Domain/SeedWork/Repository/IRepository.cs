namespace Ahsoka.Domain;

public interface IRepository<T> where T : Entity
{
    Task InsertAsync(T entity, CancellationToken cancellationToken);
    Task UpdateAsync(T entity, CancellationToken cancellationToken);

    Task DeleteAsync(T entity, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
