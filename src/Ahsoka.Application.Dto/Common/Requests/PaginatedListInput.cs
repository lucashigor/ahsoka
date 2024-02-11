namespace Ahsoka.Application.Dto.Common.Requests;

public record PaginatedListInput
    (int Page, int PerPage, string? Search,
    string? Sort, SearchOrder Dir);