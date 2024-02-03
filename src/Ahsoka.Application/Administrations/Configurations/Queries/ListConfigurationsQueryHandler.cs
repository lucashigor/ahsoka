﻿using Ahsoka.Application.Dto.Administrations.Configurations.Responses;
using Ahsoka.Application.Dto.Common.Requests;
using Ahsoka.Domain.Entities.Admin.Configurations.Repository;
using Mapster;
using MediatR;

namespace Ahsoka.Application.Administrations.Configurations.Queries;

public record ListConfigurationsQuery
    (int Page, int PerPage, string? Search,
    string? Sort, SearchOrder Dir)
    : PaginatedListInput
    (Page, PerPage, Search,
    Sort, Dir), 
    IRequest<ListConfigurationsOutput>;

public class ListConfigurationsQueryHandler : IRequestHandler<ListConfigurationsQuery, ListConfigurationsOutput>
{
    private readonly IConfigurationRepository _configurationRepository;

    public ListConfigurationsQueryHandler(IConfigurationRepository configurationRepository)
    {
        _configurationRepository = configurationRepository;
    }

    public async Task<ListConfigurationsOutput> Handle(ListConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var searchOutput = await _configurationRepository.SearchAsync(
            new(
                request.Page,
                request.PerPage,
                request.Search,
                request.Sort,
                (Domain.SeedWork.Repository.ISearchableRepository.SearchOrder)request.Dir
            ),
            cancellationToken
        );

        return searchOutput.Adapt<ListConfigurationsOutput>();
    }
}