namespace Ahsoka.Domain;

public interface ISearchableRepository<T, J, R> where T : Entity<J> where R : SearchInput where J : IEquatable<J>
{
    Task<SearchOutput<T>> SearchAsync(R input, CancellationToken cancellationToken);
}
