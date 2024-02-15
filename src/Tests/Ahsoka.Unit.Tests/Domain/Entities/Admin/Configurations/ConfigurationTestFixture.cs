using Ahsoka.Application.Dto.Administrations.Configurations.Requests;
using Ahsoka.Domain.Entities.Admin.Configurations;
using Ahsoka.TestsUtil;

namespace Ahsoka.Unit.Tests.Domain.Entities.Admin.Configurations;

public class ConfigurationTestFixture
{
    public BaseConfiguration GetDtoBaseConfiguration(string? name)
        => GetDtoBaseConfiguration(name, null!, null!, null!, null!);
    public BaseConfiguration GetDtoBaseConfiguration(string? name, string? value)
        => GetDtoBaseConfiguration(name, value, null!, null!, null!);
    public BaseConfiguration GetDtoBaseConfiguration(string? name, 
        string? value,
        string? description,
        DateTime? startDate,
        DateTime? expireDate)
        => new(
            Name: name ?? ConfigurationFixture.GetValidName(),
            Value: value ?? ConfigurationFixture.GetValidValue(),
            Description: description ?? ConfigurationFixture.GetValidDescription(),
            StartDate: startDate ?? ConfigurationFixture.GetValidStartDate(ConfigurationState.Awaiting),
            ExpireDate: expireDate ?? ConfigurationFixture.GetValidExpireDate(ConfigurationState.Awaiting)
                );

    [CollectionDefinition(nameof(ConfigurationTestFixture))]
    public class ConfigurationTestFixtureCollection : ICollectionFixture<ConfigurationTestFixture>
    {
    }
}
