using Ahsoka.Domain.Entities.Admin.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace Ahsoka.Infrastructure.Repositories.Context;

public class PrincipalContextFactory : IDesignTimeDbContextFactory<PrincipalContext>
{
    public PrincipalContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PrincipalContext>();
        optionsBuilder.UseNpgsql("inmemory");

        return new PrincipalContext(optionsBuilder.Options);
    }
}

public partial class PrincipalContext(DbContextOptions<PrincipalContext> options) : DbContext(options)
{
    public DbSet<Configuration> Configuration => Set<Configuration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.GetForeignKeys()
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                .ToList()
                .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);

            entityType.GetProperties()
                .Where(p => p.ClrType == typeof(string))
                .ToList()
                .ForEach(p => p.SetMaxLength(255));
        }


        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public void Upsert<T>(T entity) where T : class
    {
        if (Entry(entity).State == EntityState.Detached)
            Set<T>().Add(entity);
    }

    public void UpsertRange<T>(IEnumerable<T> entities) where T : class
    {
        foreach (var entity in entities)
        {
            if (Entry(entity).State == EntityState.Detached)
            {
                Set<T>().Add(entity);
            }
        }
    }
}
