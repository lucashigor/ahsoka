namespace Ahsoka.Domain;

public record SearchInput(int Page, int PerPage, string? Search, string? OrderBy, SearchOrder Order);