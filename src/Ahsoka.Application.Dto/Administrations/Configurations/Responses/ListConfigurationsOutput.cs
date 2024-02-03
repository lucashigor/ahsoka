using Ahsoka.Application.Dto.Common.Responses;

namespace Ahsoka.Application.Dto.Administrations.Configurations.Responses;

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
