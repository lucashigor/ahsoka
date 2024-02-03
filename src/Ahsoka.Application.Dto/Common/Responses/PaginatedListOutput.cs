namespace Ahsoka.Application.Dto.Common.Responses;

public record PaginatedListOutput<T>
{
    public int Page { get; }
    public int PerPage { get; }
    public int Total { get; }
    public IReadOnlyList<T>? Items { get; }

    protected PaginatedListOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<T> items)
    {
        Page = page;
        PerPage = perPage;
        Total = total;
        Items = items;
    }
}
