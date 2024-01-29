namespace Ahsoka.Domain;

public interface ISearchableRepository<T, R> where T : Entity where R : SearchInput
{
    Task<SearchOutput<T>> SearchAsync(R input, CancellationToken cancellationToken);
}
