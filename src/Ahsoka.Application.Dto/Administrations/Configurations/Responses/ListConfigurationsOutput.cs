namespace Ahsoka.Application.Dto;

public record ListConfigurationsOutput
    : PaginatedListOutput<ConfigurationOutput>
{
    public ListConfigurationsOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<ConfigurationOutput> items)
        : base(page, perPage, total, items)
    {
    }
}
