namespace Ahsoka.Domain.SeedWork.Repository.ISearchableRepository;

public record SearchInput(int Page, int PerPage, string? Search, string? OrderBy, SearchOrder Order);