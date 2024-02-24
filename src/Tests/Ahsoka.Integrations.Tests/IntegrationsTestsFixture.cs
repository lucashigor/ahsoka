namespace Ahsoka.Integrations.Tests;

using Ahsoka.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

public class IntegrationsTestsFixture : IAsyncLifetime
{
    private static readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
    .WithImage("postgres:14.3")
    .WithPassword("P@55w0rd")
    .Build();

    public static DbContextOptions<PrincipalContext> CreateDatabase()
        => new DbContextOptionsBuilder<PrincipalContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .Options;

    public Task DisposeAsync() => _dbContainer.StopAsync();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using var context = new PrincipalContext(CreateDatabase());
        context.Database.Migrate();
    }
}