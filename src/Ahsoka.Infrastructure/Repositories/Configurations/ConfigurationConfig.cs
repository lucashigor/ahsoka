using Ahsoka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ahsoka.Infrastructure;

public record ConfigurationConfig : IEntityTypeConfiguration<Configuration>
{
    public void Configure(EntityTypeBuilder<Configuration> entity)
    {
        entity.ToTable(nameof(Configuration), SchemasNames.ahsoka);
        entity.HasKey(k => k.Id);
        entity.Property(k => k.Name).HasMaxLength(100);
        entity.Property(k => k.Value).HasMaxLength(2500);
        entity.Property(k => k.Description).HasMaxLength(1000);
        entity.Ignore(k => k.Events);
        entity.Ignore(k => k.Notifications);

        entity.Property(k => k.Id)
        .HasConversion(
            id => id!.Value,
            value => (ConfigurationId)ConfigurationId.Load(value)
        );
    }
}
